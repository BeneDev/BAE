using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour {

    [SerializeField] Button highlightedButton;
    AudioSource aS;

    private void OnEnable()
    {
        aS = GetComponent<AudioSource>();
        aS.Play();
        Invoke("SelectButton", 1f);
    }

    void SelectButton()
    {
        highlightedButton.Select();
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
