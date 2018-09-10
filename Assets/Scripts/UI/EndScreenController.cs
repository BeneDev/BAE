using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour {

    [SerializeField] Button highlightedButton;

    private void OnEnable()
    {
        Invoke("SelectButton", 1f);
    }

    void SelectButton()
    {
        highlightedButton.Select();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
