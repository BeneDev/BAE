using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {

    [SerializeField] Toggle highlightedElement;

    Dictionary<int, int[]> resolutionDict = new Dictionary<int, int[]>();

    private void Awake()
    {
        resolutionDict.Add(0, new int[] { 640, 480 });
        resolutionDict.Add(1, new int[] { 800, 600 });
        resolutionDict.Add(2, new int[] { 1280, 720 });
        resolutionDict.Add(3, new int[] { 1360, 768 });
        resolutionDict.Add(4, new int[] { 1600, 900 });
        resolutionDict.Add(5, new int[] { 1920, 1080 });
    }

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

    // Make the game run in windowed mode or not, depending on the value
    public void OnWindowedToggleChanged(bool value)
    {
        Screen.fullScreen = !value;
    }

    // Set the resolution based on the value of the dropdown menu
    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutionDict[resolutionIndex][0], resolutionDict[resolutionIndex][1], Screen.fullScreen);
    }

    // Set the Graphics quality based on the dropdown menu value
    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

}
