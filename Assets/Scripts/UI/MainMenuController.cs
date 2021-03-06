﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MainMenuController : MonoBehaviour {

    public bool OptionsOpened
    {
        get
        {
            return optionsOpened;
        }
        set
        {
            optionsOpened = value;
        }
    }

    [SerializeField] Button highlightedButton;

    AudioSource aS;

    bool optionsOpened = false;

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
        if(optionsOpened)
        {
            GameManager.Instance.FadeOptionsOut();
        }
    }

    public void OpenOptions()
    {
        GameManager.Instance.PlayButtonClick();
        GameManager.Instance.FadeOptionsIn();
        optionsOpened = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
