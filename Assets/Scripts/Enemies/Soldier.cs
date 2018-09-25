using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The soldier makes unpredictable rolls to evade the hands smashdowns
/// </summary>
public class Soldier : EnemyController {

    [SerializeField] float dodgeDuration = 1f;
    [SerializeField] float dodgeSpeedMultiplier = 3f;

    protected override void Update()
    {
        if (GameManager.Instance.IsPaused)
        {
            if (agent)
            {
                agent.isStopped = true;
            }
            anim.speed = 0f;
            return;
        }
        else if (agent)
        {
            if (agent.isStopped)
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
        if (distanceToDestination < turnAroundDistance)
        {
            toDestination = false;
            if (hasEnergy <= 0)
            {
                StealEnergy();
            }
            if (hasEnergy > 0)
            {
                Invoke("ToSpawn", startWalkingDelay);
            }
        }

        //check if Enemy is at SpawnPoint
        if (toDestination == false && distanceToSpawn < turnAroundDistance)
        {
            toDestination = true;
            toWeakSpotCounter = 0f;
            if (anim)
            {
                anim.SetTrigger("LoseEnergy");
            }
            weakSpotCon.EnergyLostForever(hasEnergy);
            hasEnergy = 0;
        }

        toWeakSpotCounter += Time.deltaTime;
        if (toWeakSpotCounter > timeWalkingToWeakSpotUntilDead)
        {
            Destroy(gameObject);
        }

    }

    protected override void ReactToHandSmashNearby(Vector3 impactPos)
    {
        if ((impactPos - transform.position).magnitude < getSlowedDownThreshold)
        {
            StartCoroutine(DodgeRoll(dodgeDuration, dodgeSpeedMultiplier));
        }
    }

    protected IEnumerator DodgeRoll(float rollDuration, float rollSpeedMultiplier)
    {
        if (!agent) { yield break; }
        anim.SetTrigger("Shock");
        agent.speed = normalSpeed * rollSpeedMultiplier;
        yield return new WaitForSeconds(rollDuration);
        agent.speed = normalSpeed;
    }
}
