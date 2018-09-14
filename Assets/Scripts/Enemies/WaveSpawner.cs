using System.Collections;
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
                OnWaveChanged(nextWave);
            }
        }
    }

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    public System.Action<int> OnWaveChanged;
    public System.Action OnWaveCleared;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip[] musicClips;

    Dictionary<string, int> musicDic = new Dictionary<string, int>();
    
    private int nextWave = 1;
    [SerializeField] float nextWaveWeight = 5f;
    [SerializeField] float weightGainPerWave = 3f;
    [SerializeField] float weightUntilBigGuys = 10f;

    [SerializeField] GameObject[] enemies;
    [SerializeField] GameObject bird;
    [Range(0.05f, 1f)] float chanceToSpawnBird = 0f;

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    string[] handAnimTriggerNames = new string[7];
    [SerializeField] Animator handLeftAnim;
    [SerializeField] Animator handRightAnim;

    bool canDoAnimation = false;

    void Awake()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.Log("No spawn points referenced.");
        }
        musicDic["Wave"] = 0;
        musicDic["WaveStart"] = 1;
        musicDic["WaveFinished"] = 2;
        waveCountdown = timeBetweenWaves;
        handAnimTriggerNames[0] = "Clap";
        handAnimTriggerNames[1] = "Ok";
        handAnimTriggerNames[2] = "Highfive";
        handAnimTriggerNames[3] = "Pistol";
        handAnimTriggerNames[4] = "Fingerroll";
        handAnimTriggerNames[5] = "Fistbump";
        handAnimTriggerNames[6] = "Peace";
    }

    private void OnEnable()
    {
        if(OnWaveChanged != null)
        {
            OnWaveChanged(nextWave);
        }
        InvokeRepeating("SpawnBird", 1f, 3f);
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
        if(chanceToSpawnBird > 0.02f)
        {
            chanceToSpawnBird -= 0.01f * Time.deltaTime;
        }
        if (state == SpawnState.WAITING)
        {
            //check if enemies are still alive
            if (!EnemyIsAlive())
            {
                //begin new wave
                WaveCompleted();
                if(OnWaveCleared != null)
                {
                    OnWaveCleared();
                }
                StartCoroutine(HandleHandAnimationEnabling());
                GameManager.Instance.FadeOutSound(musicSource, 1f);
                soundSource.PlayOneShot(musicClips[musicDic["WaveFinished"]]);
                return;
            }
            else
            {
                return;
            }
        }
        if(canDoAnimation && Input.GetButtonDown("Submit"))
        {
            PlayHandAnimation();
        }
        if (waveCountdown <= 0)
        {
            
            //start spawning wave
            if (state != SpawnState.SPAWNING)
            {
                if(!musicSource.isPlaying)
                {
                    soundSource.PlayOneShot(musicClips[musicDic["WaveStart"]]);
                    PlaySound(musicSource, musicClips[musicDic["Wave"]], true);
                }
                StartCoroutine(SpawnWave(nextWaveWeight));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    IEnumerator HandleHandAnimationEnabling()
    {
        canDoAnimation = true;
        //TODO show player he can do animation
        yield return new WaitForSeconds(timeBetweenWaves * 0.5f);
        canDoAnimation = false;
    }

    void PlayHandAnimation()
    {
        string trigger = handAnimTriggerNames[Random.Range(0, 7)];
        handLeftAnim.enabled = true;
        handRightAnim.enabled = true;
        handLeftAnim.SetTrigger(trigger);
        handRightAnim.SetTrigger(trigger);
        canDoAnimation = false;
    }

    public void IncreaseBirdSpawnChance(int mulitplier)
    {
        chanceToSpawnBird += 0.01f * mulitplier;
        if(chanceToSpawnBird > 0.05f)
        {
            chanceToSpawnBird = 0.05f;
        }
    }

    void SpawnBird()
    {
        if (Random.value <= chanceToSpawnBird && !GameManager.Instance.IsPaused)
        {
            Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(bird, _sp.position, _sp.rotation);
        }
    }

    void PlaySound(AudioSource source, AudioClip clip, bool isLooping = false)
    {
        source.clip = clip;
        source.loop = isLooping;
        GameManager.Instance.FadeInSound(musicSource, 0.1f, 2f);
    }

    void WaveCompleted()
    {
        Debug.Log("Wave completed!");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        NextWave++;
        nextWaveWeight += weightGainPerWave;
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
    IEnumerator SpawnWave(float waveWeight)
    {
        Debug.Log("Spawning Wave: " + nextWave);
        state = SpawnState.SPAWNING;

        float currentWeight = 0f;

        while(currentWeight < nextWaveWeight)
        {
            if(currentWeight > weightUntilBigGuys) { break; }
            currentWeight = SpawnEnemy(enemies[0], currentWeight);
            //TODO vary the amount of seconds waiting between spawns
            yield return new WaitForSeconds(1f);
        }
        while(currentWeight < nextWaveWeight)
        {
            currentWeight = SpawnEnemy(enemies[Random.Range(0, 2)], currentWeight);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    //Spawn enemy
    float SpawnEnemy(GameObject _enemy, float currentWeight)
    {
        Debug.Log("Spawning Enemy: " + _enemy.name);
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
        return currentWeight + _enemy.GetComponent<EnemyController>().WaveWeight;
    }

}
