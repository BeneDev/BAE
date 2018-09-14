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

    public float WaveWeight
    {
        get
        {
            return waveWeight;
        }
    }

    GameObject weakSpot;
    WeakSpotController weakSpotCon;

    NavMeshAgent agent;

    Vector3 spawnPosition;

    [SerializeField] float startWalkingDelay = 3f;
    [SerializeField] float turnAroundDistance = 1f;
    [SerializeField] int energyStealAmount = 3;
    [Range(0.1f, 10f), SerializeField] float waveWeight = 1f;
    int hasEnergy = 0;
    [SerializeField] Vector3 energyCarryOffset = new Vector3(0f, 1.1f, 0f);

    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip[] aClips;
    [Range(0.5f, 2f), SerializeField] float minPitch;
    [Range(1f, 3f), SerializeField] float maxPitch;

    private float distanceToDestination;
    private float distanceToSpawn;

    private bool toDestination = true;

    private Animator anim;

    private bool isWaiting = false;

    [SerializeField] bool isGreenBlooded = false;

    float toWeakSpotStartTime;
    [SerializeField] float timeWalkingToWeakSpotUntilDead = 15f;

	void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;
        weakSpot = GameObject.FindGameObjectWithTag("WeakSpot");
        weakSpotCon = weakSpot.GetComponent<WeakSpotController>();
        toWeakSpotStartTime = Time.realtimeSinceStartup;
	}
	
	
	void Update ()
    {
        if (GameManager.Instance.IsPaused)
        {
            if(agent)
            {
                agent.isStopped = true;
            }
            anim.speed = 0f;
            return;
        }
        else if (agent)
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
            toWeakSpotStartTime = Time.realtimeSinceStartup;
            if(anim)
            {
                anim.SetTrigger("LoseEnergy");
            }
            weakSpotCon.EnergyLostForever(hasEnergy);
            hasEnergy = 0;
        }

        if(Time.realtimeSinceStartup > toWeakSpotStartTime + timeWalkingToWeakSpotUntilDead)
        {
            Destroy(gameObject);
        }

    }

    public void PlayFootStep()
    {
        aSource.PlayOneShot(aClips[0]);
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
        GameManager.Instance.GetLittleEnergy(gameObject, energyCarryOffset);
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
            if(col.gameObject.GetComponent<BaseHandController>().CanKill)
            {
                StartCoroutine(PlayAtRandomPitch(aClips[1]));
                if(hasEnergy > 0)
                {
                    weakSpotCon.RegainEnergy(hasEnergy);
                }
                weakSpotCon.GainRage(energyStealAmount);
                if(isGreenBlooded)
                {
                    GameManager.Instance.GetSplatterParticle(transform.position + Vector3.up * 0.2f, "Green");
                }
                else
                {
                    GameManager.Instance.GetSplatterParticle(transform.position + Vector3.up * 0.2f, "Red");
                }
                GameManager.Instance.IncreaseKills(isGreenBlooded);
                Destroy(agent);
                Destroy(gameObject, 1f);
                Destroy(GetComponentInChildren<SkinnedMeshRenderer>());
                Destroy(GetComponent<CapsuleCollider>());
            }
        }
    }

}
