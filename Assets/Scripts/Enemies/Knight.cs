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
        base.Update();
        if(isInvincible)
        {
            agent.speed = normalSpeed * shieldUpSpeedMultiplier;
        }
    }

    protected override void ReactToHandSmashNearby(Vector3 impactPos)
    {
        if ((impactPos - transform.position).magnitude < getSlowedDownThreshold && Time.realtimeSinceStartup > lastTimeShieldUp + shieldUpCooldown && !isInvincible)
        {
            StartCoroutine(HoldShieldUp());
        }
    }

    protected IEnumerator HoldShieldUp()
    {
        if (!agent) { yield break; }

        anim.SetTrigger("Shock");
        isInvincible = true;
        agent.speed = normalSpeed * shieldUpSpeedMultiplier;
        yield return new WaitForSeconds(shieldUpDuration);
        agent.speed = normalSpeed;
        isInvincible = false;
        lastTimeShieldUp = Time.realtimeSinceStartup;
    }

    protected override void OnCollisionEnter(Collision col)
    {
        if(!isInvincible)
        {
            base.OnCollisionEnter(col);
        }
    }
}
