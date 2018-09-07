using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileManager : MonoBehaviour {

    BaseHandController handLeft;
    BaseHandController handRight;

    Vector3 startPos;
    Vector3 normalPos;
    Vector3 toImpact;

    [SerializeField] float goDownThreshold = 1f;
    [Range(3, 15), SerializeField] float isAffectedThreshold = 10f;
    [SerializeField] float waitAfterImpactMultiplier = 0.1f;
    [SerializeField] float driveAmount = 0.2f;
    [SerializeField] float driveFirstDuration = 0.1f;
    [SerializeField] float driveBackDuration = 0.5f;
    [SerializeField] float resetTime = 1f;

    private void Awake()
    {
        handLeft = GameObject.FindGameObjectWithTag("HandLeft").GetComponent<BaseHandController>();
        handRight = GameObject.FindGameObjectWithTag("HandRight").GetComponent<BaseHandController>();
        handLeft.OnHandSmashDown += Go;
        handRight.OnHandSmashDown += Go;
        handRight.OnSpecialSmashEnd += Reset;
        normalPos = transform.position;
    }

    void Go(Vector3 impactPos)
    {
        startPos = transform.position;
        toImpact = impactPos - transform.position;
        if(toImpact.magnitude <= goDownThreshold)
        {
            StartCoroutine(DriveDown());
        }
        else if(toImpact.magnitude <= isAffectedThreshold)
        {
            StartCoroutine(DriveUp(impactPos));
        }
    }

    void Reset()
    {
        StartCoroutine(BackToNormal());
    }

    IEnumerator BackToNormal()
    {
        float startY = transform.position.y;
        float differenceY = normalPos.y - startY;
        for (float t = 0; t < resetTime; t += Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x, startY + (differenceY * (t / resetTime)), transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = normalPos;
    }

    IEnumerator DriveDown()
    {
        for (float t = 0f; t < driveFirstDuration; t += Time.deltaTime)
        {
            transform.position -= Vector3.up * (driveAmount  * (isAffectedThreshold - toImpact.magnitude) * (t / driveFirstDuration));
            yield return new WaitForEndOfFrame();
        }
        for (float t = 0f; t < driveBackDuration; t += Time.deltaTime)
        {
            transform.position += Vector3.up * (driveAmount  * (isAffectedThreshold - toImpact.magnitude) * (t / driveBackDuration));
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPos;
    }

    IEnumerator DriveUp(Vector3 impactPos)
    {
        yield return new WaitForSeconds(toImpact.magnitude * waitAfterImpactMultiplier);
        for (float t = 0f; t < driveFirstDuration; t += Time.deltaTime)
        {
            transform.position += Vector3.up * (driveAmount * (isAffectedThreshold - toImpact.magnitude) * (t / driveFirstDuration));
            yield return new WaitForEndOfFrame();
        }
        for (float t = 0f; t < driveBackDuration; t += Time.deltaTime)
        {
            transform.position -= Vector3.up * (driveAmount  * (isAffectedThreshold - toImpact.magnitude) * (t / driveBackDuration));
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPos;
    }
}
