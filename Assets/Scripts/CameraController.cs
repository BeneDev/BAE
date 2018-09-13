using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Animator anim;

	// Use this for initialization
	void Awake() {
        anim = GetComponent<Animator>();
        GameManager.Instance.OnGameStarted += StartCamera;
	}
	
	void StartCamera()
    {
        anim.SetTrigger("Start");
    }
}
