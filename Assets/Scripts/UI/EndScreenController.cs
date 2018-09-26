using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour {

    [SerializeField] Button highlightedButton;
    AudioSource aS;

    [SerializeField] Text waveCountText;
    [SerializeField] Text killCountText;

    private void Awake()
    {
        GameManager.Instance.OnPlayerDied += SetEndScreenTexts;
        aS = GetComponent<AudioSource>();
    }

    void SelectButton()
    {
        highlightedButton.Select();
    }

    void SetEndScreenTexts(int waveCount, int cavemanKills, int knightKills, int soldierKills)
    {
        aS.Play();
        Invoke("SelectButton", 1f);
        waveCountText.text = "You made it to Wave " + waveCount + "!";
        killCountText.text = "Getting there, you killed " + cavemanKills + " Cavemans, " + knightKills + " Knights and " + soldierKills + " Soldiers.";
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
