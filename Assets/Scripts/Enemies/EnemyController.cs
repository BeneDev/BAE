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

    protected GameObject weakSpot;
    protected WeakSpotController weakSpotCon;

    protected NavMeshAgent agent;

    protected Vector3 spawnPosition;

    [SerializeField] protected BasePowerup[] powerUpsToSpawn;
    [SerializeField] protected float spawnRange = 2.5f;
    [Range(0, 1), SerializeField] protected float dropChance = 0.3f;

    [SerializeField] protected float startWalkingDelay = 3f;
    [SerializeField] protected float turnAroundDistance = 1f;
    [SerializeField] protected int energyStealAmount = 3;
    [Range(0.1f, 10f), SerializeField] protected float waveWeight = 1f;
    protected int hasEnergy = 0;
    [SerializeField] protected Vector3 energyCarryOffset = new Vector3(0f, 1.1f, 0f);

    [SerializeField] protected AudioSource aSource;
    [SerializeField] protected AudioClip[] aClips;
    [Range(0.5f, 2f), SerializeField] protected float minPitch;
    [Range(1f, 3f), SerializeField] protected float maxPitch;

    protected float distanceToDestination;
    protected float distanceToSpawn;

    protected bool toDestination = true;

    protected Animator anim;

    protected bool isWaiting = false;

    [SerializeField] protected bool isGreenBlooded = false;

    protected float toWeakSpotCounter = 0f;
    [SerializeField] protected float timeWalkingToWeakSpotUntilDead = 15f;

    [Tooltip("This should be the same as the one in Ground Tile Controller"), SerializeField] protected float waitAfterImpactMultiplier = 0.1f;
    [SerializeField] protected float slowedDownDuration = 1f;
    [Range(0, 1), SerializeField] protected float slowedDownSpeedMultiplier = 0.5f;
    [SerializeField] protected float getSlowedDownThreshold = 3.5f;
    protected float normalSpeed;

    protected HandLeftController handLeft;
    protected HandRightController handRight;


    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;
        weakSpot = GameObject.FindGameObjectWithTag("WeakSpot");
        weakSpotCon = weakSpot.GetComponent<WeakSpotController>();
        handRight = GameObject.FindGameObjectWithTag("HandRight").GetComponent<HandRightController>();
        handLeft = GameObject.FindGameObjectWithTag("HandLeft").GetComponent<HandLeftController>();
        handRight.OnHandSmashDown += ReactToHandSmashNearby;
        handLeft.OnHandSmashDown += ReactToHandSmashNearby;
        CalculateSpeed();
        normalSpeed = agent.speed;
    }

    private void OnDisable()
    {
        handRight.OnHandSmashDown -= ReactToHandSmashNearby;
        handLeft.OnHandSmashDown -= ReactToHandSmashNearby;
    }

    protected virtual void Update ()
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
            toWeakSpotCounter = 0f;
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
            toWeakSpotCounter = 0f;
            if(anim)
            {
                anim.SetTrigger("LoseEnergy");
            }
            weakSpotCon.EnergyLostForever(hasEnergy);
            hasEnergy = 0;
        }

        toWeakSpotCounter += Time.deltaTime;
        if(toWeakSpotCounter > timeWalkingToWeakSpotUntilDead)
        {
            Destroy(gameObject);
        }

    }

    protected void CalculateSpeed()
    {
        agent.speed *= 1 + (WaveSpawner.Instance.NextWave * 0.05f);
    }

    protected virtual void ReactToHandSmashNearby(Vector3 impactPos)
    {
        if ((impactPos - transform.position).magnitude < getSlowedDownThreshold)
        {
            StartCoroutine(React(impactPos));
        }
    }

    protected IEnumerator React(Vector3 pos)
    {
        if (!agent) { yield break; }
        anim.SetTrigger("Shock");
        Vector3 toImpact = pos - transform.position;
        yield return new WaitForSeconds(toImpact.magnitude * waitAfterImpactMultiplier);
        if(!agent) { yield break; }
        agent.speed = normalSpeed * slowedDownSpeedMultiplier;
        anim.speed = slowedDownSpeedMultiplier;
        yield return new WaitForSeconds(slowedDownDuration);
        agent.speed = normalSpeed;
        anim.speed = 1f;
    }

    protected void SpawnPowerUp(Vector3 offset)
    {
        if(powerUpsToSpawn.Length > 0)
        {
            Instantiate(powerUpsToSpawn[Random.Range(0, powerUpsToSpawn.Length)], (transform.position + (Vector3)(Random.insideUnitCircle * spawnRange)) + offset, Quaternion.identity);
        }
    }

    public void PlayFootStep()
    {
        aSource.PlayOneShot(aClips[0]);
    }

    protected void StealEnergy()
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

    protected void TakeEnergy()
    {
        GameManager.Instance.GetLittleEnergy(gameObject, energyCarryOffset);
    }

    protected void ToWeakSpot()
    {
        if(agent)
        {
            agent.SetDestination(weakSpot.transform.position);
        }
    }

    protected void ToSpawn()
    {
        if(agent)
        {
            agent.SetDestination(spawnPosition);
        }
    }

    protected IEnumerator PlayAtRandomPitch(AudioClip clip)
    {
        aSource.pitch = Random.Range(minPitch, maxPitch);
        aSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        aSource.pitch = 1f;
    }

    //check if Enemy is hit by Hand
    protected virtual void OnCollisionEnter(Collision col)
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
                weakSpotCon.GainRage(energyStealAmount / 2);
                if(isGreenBlooded)
                {
                    GameManager.Instance.GetSplatterParticle(transform.position + Vector3.up * 0.2f, "Green");
                }
                else
                {
                    GameManager.Instance.GetSplatterParticle(transform.position + Vector3.up * 0.2f, "Red");
                }
                if(Random.value < dropChance)
                {
                    SpawnPowerUp(Vector3.up * 0.6f);
                }
                GameManager.Instance.IncreaseKills(isGreenBlooded);
                Destroy(agent);
                Destroy(gameObject, 1f);
                Destroy(GetComponentInChildren<SkinnedMeshRenderer>());
                Destroy(GetComponentInChildren<MeshRenderer>());
                Destroy(GetComponent<CapsuleCollider>());
            }
        }
    }

}
