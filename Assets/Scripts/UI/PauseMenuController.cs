using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour {

    public System.Action OnContinue;

    [SerializeField] Button highlightedButton;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.SetTrigger("Show");
        Invoke("SelectButton", 1f);
    }

    void SelectButton()
    {
        highlightedButton.Select();
    }

    public void Continue()
    {
        if(OnContinue != null)
        {
            OnContinue();
        }
        anim.SetTrigger("Hide");
    }

    public void Options()
    {
        //TODO open options menu
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
