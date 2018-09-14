using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {

    [SerializeField] Toggle highlightedElement;

    private void OnEnable()
    {
        Invoke("SelectFirstElement", 0.3f);
    }

    void SelectFirstElement()
    {
        highlightedElement.Select();
    }

    public void Back()
    {
        GameManager.Instance.PlayButtonClick();
        GameManager.Instance.FadeOptionsOut();
    }

    public void OnPsInputChanged(bool isPSInput)
    {
        GameManager.Instance.IsPSInput = isPSInput;
    }

    public void OnMusicVolumeChanged(float newVolume)
    {
        AudioListener.volume = newVolume;
    }

    public void OnDeadzoneChanged(float Deadzone)
    {
        //TODO change the deadzones in input manager
    }

    public void OnControllerSensitivityChanged(float sensitivity)
    {
        //TODO change the sensibility of the control sticks
    }

}
