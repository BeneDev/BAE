using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public int HasEnergy
    {
        get
        {
            return hasEnergy;
        }
    }

    GameObject weakSpot;
    WeakSpotController weakSpotCon;

    NavMeshAgent agent;

    Vector3 spawnPosition;

    [SerializeField] float startWalkingDelay = 3f;
    [SerializeField] float turnAroundDistance = 1f;
    [SerializeField] int energyStealAmount = 3;
    int hasEnergy = 0;

    private float distanceToDestination;
    private float distanceToSpawn;

    private bool toDestination = true;

    private Animator anim;

    private bool isWaiting = false;

	void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
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
            if(hasEnergy <= 0)
            {
                StealEnergy();
            }
            if(hasEnergy > 0)
            {
                Invoke("ToSpawn", startWalkingDelay);
            }
        }

        //check if Enemy is at SpawnPoint
        if (toDestination == false && distanceToSpawn < turnAroundDistance)
        {
            toDestination = true;
            if(anim)
            {
                anim.SetTrigger("LoseEnergy");
            }
            weakSpotCon.EnergyLostForever(hasEnergy);
            hasEnergy = 0;
        }

    }

    private void StealEnergy()
    {
        hasEnergy = weakSpotCon.LoseEnergy(energyStealAmount);
        if (anim && hasEnergy > 0)
        {
            anim.SetTrigger("TakeEnergy");
            isWaiting = false;
            Invoke("TakeEnergy", 0.5f);
        }
        else if(anim && !isWaiting)
        {
            anim.SetTrigger("WaitToTake");
            isWaiting = true;
        }
    }

    void TakeEnergy()
    {
        GameManager.Instance.GetLittleEnergy(gameObject);
    }

    void ToWeakSpot()
    {
        if(agent)
        {
            agent.SetDestination(weakSpot.transform.position);
        }
    }

    void ToSpawn()
    {
        if(agent)
        {
            agent.SetDestination(spawnPosition);
        }
    }

    //check if Enemy is hit by Hand
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<BaseHandController>())
        {
            if(col.gameObject.GetComponent<BaseHandController>().CanKill)
            {
                if(hasEnergy > 0)
                {
                    weakSpotCon.RegainEnergy(hasEnergy);
                }
                GameManager.Instance.GetSplatterParticle(transform.position + Vector3.up * 0.2f);
                Destroy(gameObject);
            }
        }
    }

}
