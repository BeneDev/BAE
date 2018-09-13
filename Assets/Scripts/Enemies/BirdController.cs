using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BirdController : MonoBehaviour {

    public bool IsFinished
    {
        get
        {
            return isFinished;
        }
    }

    GameObject weakSpot;
    WeakSpotController weakSpotCon;

    NavMeshAgent agent;

    Vector3 spawnPosition;

    [SerializeField] float startWalkingDelay = 3f;
    [SerializeField] float turnAroundDistance = 1f;
    [SerializeField] int energyToGive = 3;
    [SerializeField] Vector3 energyCarryOffset;

    private float distanceToDestination;
    private float distanceToSpawn;

    private bool toDestination = true;

    bool isFinished = false;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;
        weakSpot = GameObject.FindGameObjectWithTag("WeakSpot");
        weakSpotCon = weakSpot.GetComponent<WeakSpotController>();
        GameManager.Instance.GetLittleEnergy(gameObject, energyCarryOffset);
    }

    void Update()
    {
        distanceToDestination = Vector3.Distance(transform.position, weakSpot.transform.position);
        distanceToSpawn = Vector3.Distance(transform.position, spawnPosition);

        if (toDestination == true)
        {
            if (agent)
            {
                agent.SetDestination(weakSpot.transform.position);
            }
        }

        //check if Enemy is at WeakSpot
        if (distanceToDestination < turnAroundDistance)
        {
            if(toDestination)
            {
                weakSpotCon.GainEnergy(energyToGive);
            }
            anim.SetTrigger("GiveEnergy");
            isFinished = true;
            if (agent)
            {
                agent.SetDestination(spawnPosition);
            }
            toDestination = false;
        }

        //check if Enemy is at SpawnPoint
        if (toDestination == false && distanceToSpawn < turnAroundDistance)
        {
            Destroy(gameObject);
        }
    }

    //check if Enemy is hit by Hand
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<BaseHandController>())
        {
            if (col.gameObject.GetComponent<BaseHandController>().CanKill)
            {
                GameManager.Instance.GetSplatterParticle(transform.position + Vector3.up * 0.2f);
                Destroy(gameObject);
            }
        }
    }
}
