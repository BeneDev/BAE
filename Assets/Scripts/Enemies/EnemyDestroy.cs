using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroy : MonoBehaviour {

    //check if Enemy is hit by Hand
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "HandRight" || col.gameObject.name == "HandLeft")
        {
            Destroy(gameObject);
        }
    }
}
