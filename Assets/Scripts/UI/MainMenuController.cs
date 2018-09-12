using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    [SerializeField] Button highlightedButton;

    private void Start()
    {
        highlightedButton.Select();
    }

    public void Play()
    {
        GameManager.Instance.PlayGame();
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
