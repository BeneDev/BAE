using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveCountController : MonoBehaviour {

    [SerializeField] Text waveCountText;

    private void Awake()
    {
        WaveSpawner.Instance.OnWaveChanged += ChangeWaveCountText;   
    }

    void ChangeWaveCountText(int newCount)
    {
        waveCountText.text = "Wave: " + newCount;
    }

}
