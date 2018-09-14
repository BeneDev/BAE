using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] Text waveCountText;

    [SerializeField] Slider rageBar;

    Animator anim;

    private void Awake()
    {
        WaveSpawner.Instance.OnWaveChanged += ChangeWaveCountText;
        WaveSpawner.Instance.OnWaveCleared += WaveCleared;
        WeakSpotController.Instance.OnRageMeterChanged += ChangeRageBar;
        anim = GetComponent<Animator>();
    }

    void ChangeWaveCountText(int newCount)
    {
        waveCountText.text = "Wave " + newCount;
        anim.SetTrigger("BlinkWave");
    }

    void WaveCleared()
    {
        anim.SetTrigger("WaveCleared");
    }

    void ChangeRageBar(int value, int maxValue)
    {
        rageBar.maxValue = maxValue;
        rageBar.value = value;
    }

}
