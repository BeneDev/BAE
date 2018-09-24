using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The knight can put up his shield, which makes him invincible, but slower.
/// </summary>
public class Knight : EnemyController {

    bool isInvincible = false;
    [SerializeField] float shieldUpSpeedMultiplier = 0.5f;
    [SerializeField] float shieldUpDuration = 2f;
    [SerializeField] float shieldUpCooldown = 5f;
    float lastTimeShieldUp = 0f;

    protected override void Update()
    {
        if(!isInvincible)
        {
            base.Update();
        }
        if(isInvincible)
        {
            agent.speed = normalSpeed * shieldUpSpeedMultiplier;
        }
    }

    protected override void ReactToHandSmashNearby(Vector3 impactPos)
    {
        if ((impactPos - transform.position).magnitude < getSlowedDownThreshold && Time.realtimeSinceStartup > lastTimeShieldUp + shieldUpCooldown && !isInvincible)
        {
            isInvincible = true;
            StartCoroutine(HoldShieldUp(impactPos));
        }
    }

    protected IEnumerator HoldShieldUp(Vector3 impactPos)
    {
        if (!agent) { yield break; }
        Vector3 originalDestination = agent.destination;
        Vector3 toImpact = impactPos - transform.position;
        agent.SetDestination(transform.position - toImpact * 2f);
        anim.SetTrigger("Shock");
        agent.speed = normalSpeed * shieldUpSpeedMultiplier;
        yield return new WaitForSeconds(shieldUpDuration);
        agent.speed = normalSpeed;
        anim.SetTrigger("ShieldDown");
        lastTimeShieldUp = Time.realtimeSinceStartup;
        isInvincible = false;
        agent.SetDestination(originalDestination);
    }

    protected override void OnCollisionEnter(Collision col)
    {
        if(!isInvincible)
        {
            base.OnCollisionEnter(col);
        }
    }
}
