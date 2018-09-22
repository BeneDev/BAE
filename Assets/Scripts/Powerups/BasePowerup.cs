using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePowerup : MonoBehaviour {

    [SerializeField] protected float duration = 5f;

    protected HandLeftController handLeft;
    protected HandRightController handRight;

    protected MeshRenderer rend;
    protected SphereCollider coll;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        coll = GetComponent<SphereCollider>();
    }

    protected virtual void Use()
    {
        Destroy(rend);
        Destroy(coll);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "HandLeft")
        {
            handLeft = other.gameObject.GetComponent<HandLeftController>();
            Use();
        }
        else if(other.gameObject.tag == "HandRight")
        {
            handRight = other.gameObject.GetComponent<HandRightController>();
            Use();
        }
    }
}
