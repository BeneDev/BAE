﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour {

    public System.Action OnContinue;

    [SerializeField] Button highlightedButton;

    private void OnEnable()
    {
        Invoke("SelectButton", 0.3f);
    }

    void SelectButton()
    {
        highlightedButton.Select();
    }

    public void Continue()
    {
        GameManager.Instance.PlayButtonClick();
        if(OnContinue != null)
        {
            OnContinue();
        }
    }

    public void Options()
    {
        GameManager.Instance.PlayButtonClick();
        GameManager.Instance.FadeOptionsIn();
    }

    public void MainMenu()
    {
        GameManager.Instance.PlayButtonClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
