using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHandController : MonoBehaviour {

    public event System.Action<Vector3> OnHandSmashDown;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float smashSpeed = 2f;
    [SerializeField] protected float resetDistance = 0.01f;
    [SerializeField] protected float resetTime = 2f;
    [SerializeField] protected float resetSpeed = 1f;
    [SerializeField] protected float smashTravelDistanceY = 0.5f;
    [SerializeField] protected float smashDownDuration = 0.2f;
    
    protected float t1;
    protected float t2;
    protected float startTime;
    protected float smashLength;

    protected Rigidbody rBody;

    protected bool canSmash = true;

    protected Vector3 moveInput;
    protected Vector3 smashPositionStart;
    protected Vector3 moveVelocity;

    protected Vector2 triggerInput;

    protected virtual void Awake()
    {
        rBody = GetComponent<Rigidbody>();
    }

    //smash ground
    protected virtual void smash()
    {
        canSmash = false;

        // freeze Hand position
        rBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        //smashPositionStart = transform.position;
        //smashPositionEnd = smashPositionStart;
        //smashPositionEnd.y = 0.5f;

        StartCoroutine(SmashDown());

        //Debug.Log("Current Position is " + smashPositionStart + "Smash Position is " + smashPositionEnd);

        //t1 += Time.deltaTime / smashSpeed;
        //transform.position = Vector3.Lerp(smashPositionStart, smashPositionEnd, t1);

        //transform.position = Vector3.Lerp(smashPositionStart, smashPositionEnd, smashSpeed * Time.deltaTime);

        //transform.position = smashPositionEnd;

        //Invoke("ResetAfterSmash", resetTime);
    }

    IEnumerator SmashDown()
    {
        smashPositionStart = transform.position;
        for (float t = 0f; t < smashDownDuration; t += Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - t, transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = smashPositionStart + Vector3.down * smashTravelDistanceY;
        Invoke("ResetAfterSmash", resetTime);
    }

    //Reset after Smash (gets invoked after resetTime seconds)
    protected virtual void ResetAfterSmash()
    {
        //Debug.Log("Reset Time is Over");

        //smashPositionEnd = transform.position;
        //smashPositionReset = smashPositionEnd;
        //smashPositionReset.y = 3f;

        //t2 += Time.deltaTime / resetSpeed;
        //transform.position = Vector3.Lerp(smashPositionEnd, smashPositionReset, t2);

        transform.position = smashPositionStart;

        // un-freeze Hand position but keep rotation frozen
        rBody.constraints = RigidbodyConstraints.FreezeRotation;

        canSmash = true;
    }

}
