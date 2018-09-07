using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class BaseHandController : MonoBehaviour {

    public bool CanKill
    {
        get
        {
            return canKill;
        }
    }

    public event System.Action<Vector3> OnHandSmashDown;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float smashSpeed = 2f;
    [SerializeField] protected float resetDistance = 0.01f;
    [SerializeField] protected float resetTime = 2f;
    [SerializeField] protected float resetSpeed = 1f;
    [SerializeField] protected float smashTravelDistanceY = 0.5f;
    [SerializeField] protected float smashDownDuration = 0.2f;
    [SerializeField] protected float smashCamShakeAmount = 0.3f;
    [SerializeField] protected float smashCamShakeDuration = 0.15f;
    [SerializeField] protected float smashGamePadRumbleDuration = 0.2f;

    [SerializeField] ParticleSystem smashImpact;
    
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

    protected bool canKill = false;

    protected CameraShake camShake;

    protected GamePadState padState;

    protected virtual void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        camShake = Camera.main.GetComponent<CameraShake>();
    }

    //smash ground
    protected virtual void smash()
    {
        canSmash = false;
        rBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        StartCoroutine(SmashDown());
    }

    IEnumerator SmashDown()
    {
        canKill = true;
        smashPositionStart = transform.position;
        for (float t = 0f; t < smashDownDuration; t += Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - t, transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = smashPositionStart + Vector3.down * smashTravelDistanceY;
        if(OnHandSmashDown != null)
        {
            OnHandSmashDown(transform.position);
        }
        Invoke("ResetAfterSmash", resetTime);
        if(smashImpact)
        {
            smashImpact.Play();
        }
        StartCoroutine(VibrateController(smashGamePadRumbleDuration));
        camShake.shakeAmount = smashCamShakeAmount;
        camShake.shakeDuration = smashCamShakeDuration;
        yield return new WaitForSeconds(0.2f);
        canKill = false;
    }

    //IEnumerator SpezialSmash()
    //{

    //}

    IEnumerator VibrateController(float duration)
    {
        GamePad.SetVibration(PlayerIndex.One, padState.Triggers.Left, padState.Triggers.Right);
        yield return new WaitForSeconds(duration);
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
    }

    //Reset after Smash (gets invoked after resetTime seconds)
    protected virtual void ResetAfterSmash()
    {
        transform.position = smashPositionStart;

        // un-freeze Hand position but keep rotation frozen
        rBody.constraints = RigidbodyConstraints.FreezeRotation;

        canSmash = true;
    }

}
