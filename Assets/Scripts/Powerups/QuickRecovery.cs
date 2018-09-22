using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRecovery : BasePowerup {

    protected override void Use()
    {
        base.Use();
        if(handRight)
        {
            StartCoroutine(Go(handRight));
        }
        else if(handLeft)
        {
            StartCoroutine(Go(handLeft));
        }
    }

    IEnumerator Go(BaseHandController targetHand)
    {
        float normalResetTime = targetHand.ResetTime;
        targetHand.ResetTime = normalResetTime * 0.3f;
        yield return new WaitForSeconds(duration);
        targetHand.ResetTime = normalResetTime;
        Destroy(gameObject);
    }
}
