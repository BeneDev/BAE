using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRecovery : BasePowerup {

    float normalResetTime;

    private void OnDisable()
    {
        StopAllCoroutines();
        if(handLeft)
        {
            handLeft.ResetTime = normalResetTime;
        }
        else if(handRight)
        {
            handRight.ResetTime = normalResetTime;
        }
    }

    protected override void Use()
    {
        base.Use();
        if(handRight)
        {
            handRight.CurrentPowerup = gameObject;
            StartCoroutine(Go(handRight));
        }
        else if(handLeft)
        {
            handLeft.CurrentPowerup = gameObject;
            StartCoroutine(Go(handLeft));
        }
    }

    IEnumerator Go(BaseHandController targetHand)
    {
        normalResetTime = targetHand.ResetTime;
        targetHand.ResetTime = normalResetTime * 0.3f;
        yield return new WaitForSeconds(duration);
        targetHand.ResetTime = normalResetTime;
        Destroy(gameObject);
    }
}
