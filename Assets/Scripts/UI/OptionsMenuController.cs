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
        GameManager.Instance.FadeOptionsOut();
    }

}
