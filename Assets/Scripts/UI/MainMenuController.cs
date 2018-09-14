using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    [SerializeField] Button highlightedButton;

    AudioSource aS;

    private void Start()
    {
        highlightedButton.Select();
        aS = GetComponent<AudioSource>();
    }

    public void Play()
    {
        GameManager.Instance.PlayGame();
        GameManager.Instance.FadeOutSound(aS, 1f);
    }

    public void Options()
    {
        //TODO open options menu
    }

    public void Quit()
    {
        Application.Quit();
    }
}
