using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MainMenuController : MonoBehaviour {

    [SerializeField] Button highlightedButton;

    AudioSource aS;

    private void Start()
    {
        highlightedButton.Select();
        aS = GetComponent<AudioSource>();
        GameManager.Instance.OnResumeToMainMenu += OnResume;
    }

    void OnResume()
    {
        highlightedButton.Select();
    }

    public void Play()
    {
        GameManager.Instance.PlayButtonClick();
        GameManager.Instance.PlayGame();
        GameManager.Instance.FadeOutSound(aS, 1f);
    }

    public void Options()
    {
        // THIS BUGS OUT AGAIN AND AGAIN
        // Do i really need to change this file with comments to make the button work?!
        GameManager.Instance.PlayButtonClick();
        GameManager.Instance.FadeOptionsIn();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
