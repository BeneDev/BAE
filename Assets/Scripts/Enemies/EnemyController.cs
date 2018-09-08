using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    GameObject weakSpot;
    WeakSpotController weakSpotCon;

    [SerializeField] NavMeshAgent agent;

    Vector3 spawnPosition;

    [SerializeField] float startWalkingDelay = 3f;
    [SerializeField] float turnAroundDistance = 1f;
    [SerializeField] int energyStealAmount = 3;
    bool hasEnergy = false;

    private float distanceToDestination;
    private float distanceToSpawn;

    private bool toDestination = true;

    private Animator anim;

	void Awake()
    {
        anim = GetComponent<Animator>();
        spawnPosition = transform.position;
        weakSpot = GameObject.FindGameObjectWithTag("WeakSpot");
        weakSpotCon = weakSpot.GetComponent<WeakSpotController>();
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
            if(!hasEnergy)
            {
                StealEnergy();
            }
            Invoke("ToSpawn", startWalkingDelay);
        }

        //check if Enemy is at SpawnPoint
        if (toDestination == false && distanceToSpawn < turnAroundDistance)
        {
            toDestination = true;
            if(anim)
            {
                anim.SetTrigger("LoseEnergy");
            }
            hasEnergy = false;
        }

    }

    private void StealEnergy()
    {
        weakSpotCon.LoseEnergy(energyStealAmount);
        if (anim)
        {
            anim.SetTrigger("TakeEnergy");
        }
        hasEnergy = true;
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
