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

    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip[] aClips;
    [Range(0.5f, 2f), SerializeField] float minPitch;
    [Range(1f, 3f), SerializeField] float maxPitch;

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
        if(GameManager.Instance.IsPaused)
        {
            if(agent)
            {
                agent.isStopped = true;
            }
            anim.speed = 0f;
            return;
        }
        else if(agent)
        {
            if(agent.isStopped)
            {
                agent.isStopped = false;
                anim.speed = 1f;
            }
        }
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

    IEnumerator PlayAtRandomPitch(AudioClip clip)
    {
        aSource.pitch = Random.Range(minPitch, maxPitch);
        aSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        aSource.pitch = 1f;
    }

    //check if Enemy is hit by Hand
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<BaseHandController>())
        {
            if (col.gameObject.GetComponent<BaseHandController>().CanKill)
            {
                GameManager.Instance.GetSplatterParticle(transform.position + Vector3.up * 0.2f, "Blue");
                StartCoroutine(PlayAtRandomPitch(aClips[0]));
                Destroy(agent);
                Destroy(gameObject, 1f);
                Destroy(GetComponentInChildren<SkinnedMeshRenderer>());
                Destroy(GetComponent<CapsuleCollider>());
            }
        }
    }
}
