using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuController : MonoBehaviour {

    //[SerializeField] 

    private void OnEnable()
    {
        Invoke("SelectFirstElement", 0.3f);
    }

    void SelectFirstElement()
    {

    }

    public void Back()
    {

    }

}
