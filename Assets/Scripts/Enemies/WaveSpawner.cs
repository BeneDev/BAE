﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : Singleton<WaveSpawner> {

    private int NextWave
    {
        get
        {
            return nextWave;
        }
        set
        {
            nextWave = value;
            if (OnWaveChanged != null)
            {
                OnWaveChanged(nextWave + 1);
            }
        }
    }

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public EnemyToSpawn[] enemies;
        public float spawnRate;
    }

    [System.Serializable] 
    public struct EnemyToSpawn
    {
        public GameObject enemy;
        public int count;
    }

    public System.Action<int> OnWaveChanged;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip[] musicClips;

    Dictionary<string, int> musicDic = new Dictionary<string, int>();

    public Wave[] waves;
    private int nextWave = 0;

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    void Awake()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.Log("No spawn points referenced.");
        }
        musicDic["Wave"] = 0;
        waveCountdown = timeBetweenWaves;
    }

    private void OnEnable()
    {
        if(OnWaveChanged != null)
        {
            OnWaveChanged(nextWave + 1);
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.FadeOutSound(musicSource, 1f);
    }

    void Update()
    {
        if (GameManager.Instance.IsPaused)
        {
            return;
        }
        if (state == SpawnState.WAITING)
        {
            //check if enemies are still alive
            if (!EnemyIsAlive())
            {
                //begin new wave
                WaveCompleted();
                return;
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            
            //start spawning wave
            if (state != SpawnState.SPAWNING)
            {
                if(!musicSource.isPlaying)
                {
                    PlaySound(musicSource, musicClips[musicDic["Wave"]], true);
                }
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    void PlaySound(AudioSource source, AudioClip clip, bool isLooping = false)
    {
        source.clip = clip;
        source.loop = isLooping;
        GameManager.Instance.FadeInSound(musicSource, 0.5f, 1f);
    }

    void WaveCompleted()
    {
        Debug.Log("Wave completed!");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            NextWave = 0;
            Debug.Log("ALL WAVES COMPLETE! Looping...");
            //can add wave-stat multiplier etc.
        }
        else
        {
            NextWave++;
        }
    }

    //check if enemies are still alive
    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;

        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    //spawn a wave
    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.name);
        state = SpawnState.SPAWNING;

        for(int i = 0; i < _wave.enemies.Length; i++)
        {
            for (int j = 0; j < _wave.enemies[i].count; j++)
            {
                SpawnEnemy(_wave.enemies[i].enemy);
                yield return new WaitForSeconds(1f / _wave.spawnRate);
            }
        }

        state = SpawnState.WAITING;

        yield break;
    }

    //Spawn enemy
    void SpawnEnemy(GameObject _enemy)
    {
        Debug.Log("Spawning Enemy: " + _enemy.name);
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }

}
