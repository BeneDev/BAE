using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasterMovement : BasePowerup
{

    float normalSpeed;
    [Range(1, 3), SerializeField] float speedBoostMultiplier = 1.5f;

    private void OnDisable()
    {
        StopAllCoroutines();
        if (handLeft)
        {
            handLeft.Speed = normalSpeed;
        }
        else if (handRight)
        {
            handRight.Speed = normalSpeed;
        }
    }

    protected override void Use()
    {
        base.Use();
        if (handRight)
        {
            handRight.CurrentPowerup = gameObject;
            StartCoroutine(Go(handRight));
        }
        else if (handLeft)
        {
            handLeft.CurrentPowerup = gameObject;
            StartCoroutine(Go(handLeft));
        }
    }

    IEnumerator Go(BaseHandController targetHand)
    {
        targetHand.ChangePowerup(icon);
        normalSpeed = targetHand.Speed;
        targetHand.Speed = normalSpeed * speedBoostMultiplier;
        yield return new WaitForSeconds(duration);
        targetHand.Speed = normalSpeed;
        targetHand.ChangePowerup();
        Destroy(gameObject);
    }
}