using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileManager : MonoBehaviour {

    BaseHandController handLeft;
    BaseHandController handRight;

    Vector3 startPos;
    Vector3 toImpact;

    [SerializeField] float goDownThreshold = 1f;
    [SerializeField] float waitAfterImpactMultiplier = 0.1f;
    [SerializeField] float driveAmount = 0.2f;
    [SerializeField] float driveFirstDuration = 0.1f;
    [SerializeField] float driveBackDuration = 0.5f;

    private void Awake()
    {
        handLeft = GameObject.FindGameObjectWithTag("HandLeft").GetComponent<BaseHandController>();
        handRight = GameObject.FindGameObjectWithTag("HandRight").GetComponent<BaseHandController>();
        handLeft.OnHandSmashDown += Go;
        handRight.OnHandSmashDown += Go;
    }

    void Go(Vector3 impactPos)
    {
        startPos = transform.position;
        toImpact = impactPos - transform.position;
        if(toImpact.magnitude <= goDownThreshold)
        {
            StartCoroutine(DriveDown());
        }
        else
        {
            StartCoroutine(DriveUp(impactPos));
        }
    }

    IEnumerator DriveDown()
    {
        for (float t = 0f; t < driveFirstDuration; t += Time.deltaTime)
        {
            transform.position -= Vector3.up * driveAmount * t / driveFirstDuration;
            yield return new WaitForEndOfFrame();
        }
        for (float t = 0f; t < driveBackDuration; t += Time.deltaTime)
        {
            transform.position += Vector3.up * driveAmount * t / driveBackDuration;
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPos;
    }

    IEnumerator DriveUp(Vector3 impactPos)
    {
        yield return new WaitForSeconds(toImpact.magnitude * waitAfterImpactMultiplier);
        for (float t = 0f; t < driveFirstDuration; t += Time.deltaTime)
        {
            transform.position += Vector3.up * driveAmount * t / driveFirstDuration;
            yield return new WaitForEndOfFrame();
        }
        for (float t = 0f; t < driveBackDuration; t += Time.deltaTime)
        {
            transform.position -= Vector3.up * driveAmount * t / driveBackDuration;
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPos;
    }
}
