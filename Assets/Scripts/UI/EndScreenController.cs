using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour {

    [SerializeField] Button highlightedButton;
    AudioSource aS;

    [SerializeField] Text waveCountText;

    private void Awake()
    {
        GameManager.Instance.OnPlayerDied += SetWaveCountText;
        aS = GetComponent<AudioSource>();
    }

    void SelectButton()
    {
        highlightedButton.Select();
    }

    void SetWaveCountText(int waveCount)
    {
        aS.Play();
        Invoke("SelectButton", 1f);
        waveCountText.text = "You made it to Wave " + waveCount + "!";
    }

    public void PlayAgain()
    {
        GameManager.Instance.PlayButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
