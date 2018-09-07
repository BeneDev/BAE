using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public GameObject weakSpot;
    public NavMeshAgent agent;

    public Vector3 spawnPosition;

    public float startWalkingDelay = 3f;
    public float turnAroundDistance = 1f;

    private float distanceToDestination;
    private float distanceToSpawn;

    private bool toDestination = true;

    private Animator anim;

	void Awake()
    {
        anim = GetComponent<Animator>();
        spawnPosition = transform.position;
        weakSpot = GameObject.Find("WeakSpot");
	}
	
	
	void Update () {

        //Debug.Log("spawn position: " + spawnPosition + "current position: " + transform.position);

        distanceToDestination = Vector3.Distance(transform.position, weakSpot.transform.position);
        distanceToSpawn = Vector3.Distance(transform.position, spawnPosition);

        //Debug.Log("Distance to Weakspot: " + distanceToDestination);

        if (toDestination == true)
        {
            ToWeakSpot();
        }

        //check if Enemy is at WeakSpot
        if(distanceToDestination < turnAroundDistance)
        {
            toDestination = false;
            if(anim)
            {
                anim.SetTrigger("TakeEnergy");
            }
            Invoke("ToSpawn", startWalkingDelay);
        }

        //check if Enemy is at SpawnPoint
        if (toDestination == false && distanceToSpawn < turnAroundDistance)
        {
            toDestination = true;
            if(anim)
            {
                anim.ResetTrigger("TakeEnergy");
                anim.SetTrigger("LoseEnergy");
            }
        }

    }

    void ToWeakSpot()
    {
        agent.SetDestination(weakSpot.transform.position);
    }

    void ToSpawn()
    {
        agent.SetDestination(spawnPosition);
    }

    //check if Enemy is hit by Hand
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<BaseHandController>())
        {
            if(col.gameObject.GetComponent<BaseHandController>().CanKill)
            {
                Destroy(gameObject);
            }
        }
    }

}
