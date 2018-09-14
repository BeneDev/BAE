using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] Text waveCountText;

    [SerializeField] Slider rageBar;

    Animator anim;

    [SerializeField] Image doAnimationImage;
    [SerializeField] GameObject rageInstructions;

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
        StartCoroutine(ShowDoAnimationForSeconds(2.5f));
    }

    void ManageRageInstructions(bool enable)
    {
        rageInstructions.SetActive(enable);
    }

    IEnumerator ShowDoAnimationForSeconds(float seconds)
    {
        doAnimationImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        doAnimationImage.gameObject.SetActive(false);
    }

    void ChangeRageBar(int value, int maxValue)
    {
        rageBar.maxValue = maxValue;
        rageBar.value = value;
        if(value >= maxValue)
        {
            ManageRageInstructions(true);
        }
        else
        {
            ManageRageInstructions(false);
        }
    }

}
