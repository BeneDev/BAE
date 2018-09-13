using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] Text waveCountText;

    [SerializeField] Slider rageBar;

    private void Awake()
    {
        WaveSpawner.Instance.OnWaveChanged += ChangeWaveCountText;
        WeakSpotController.Instance.OnRageMeterChanged += ChangeRageBar;
    }

    void ChangeWaveCountText(int newCount)
    {
        waveCountText.text = "Wave " + newCount;
    }

    void ChangeRageBar(int value, int maxValue)
    {
        rageBar.maxValue = maxValue;
        rageBar.value = value;
    }

}
