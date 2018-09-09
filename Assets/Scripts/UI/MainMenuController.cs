using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

	public void Play()
    {
        GameManager.Instance.PlayGame();
    }

    public void Options()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
