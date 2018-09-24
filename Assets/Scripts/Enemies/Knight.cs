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

	// Use this for initialization
	void Start () {
		
	}

    protected override void Update()
    {
        base.Update();
        if(isInvincible)
        {
            agent.speed = normalSpeed * shieldUpSpeedMultiplier;
        }
    }

    protected override void OnCollisionEnter(Collision col)
    {
        if(!isInvincible)
        {
            base.OnCollisionEnter(col);
        }
    }
}
