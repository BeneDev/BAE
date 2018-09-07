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

    public bool CanSmash
    {
        get
        {
            return canSmash;
        }
    }

    public event System.Action<Vector3> OnHandSmashDown;
    public event System.Action OnSpecialSmashEnd;

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
    [SerializeField] protected float resetTransformDuration = 0.2f;

    [SerializeField] protected static int timesHittingWithSpecialSmash = 10;
    
    protected float t1;
    protected float t2;
    protected float startTime;
    protected float smashLength;

    protected Rigidbody rBody;

    protected bool canSmash = true;
    protected bool isInSpecialSmash = false;

    protected Vector3 moveInput;
    protected Vector3 smashPositionStart;
    protected Vector3 moveVelocity;

    protected Vector2 triggerInput;

    protected bool canKill = false;

    protected CameraShake camShake;

    protected GamePadState padState;

    protected bool isLeftStickDown = false;
    protected bool isRightStickDown = false;

    [SerializeField] int holdStickDownInputForFrames = 5;

    protected virtual void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        camShake = Camera.main.GetComponent<CameraShake>();
    }

    protected virtual void Update()
    {
        if (triggerInput.magnitude > 0.2 && canSmash && !isInSpecialSmash)
        {
            Smash();
            padState = GamePad.GetState(PlayerIndex.One);
        }
        if(Input.GetButtonDown("LeftStickDown"))
        {
            isLeftStickDown = true;
            CancelInvoke("ResetLeftStick");
            Invoke("ResetLeftStick", holdStickDownInputForFrames * Time.deltaTime);
        }
        if(Input.GetButtonDown("RightStickDown"))
        {
            isRightStickDown = true;
            CancelInvoke("ResetRightStick");
            Invoke("ResetRightStick", holdStickDownInputForFrames * Time.deltaTime);
        }
    }

    void ResetRightStick()
    {
        isRightStickDown = false;
    }

    void ResetLeftStick()
    {
        isLeftStickDown = false;
    }

    //smash ground
    protected virtual void Smash()
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
            transform.position = new Vector3(transform.position.x, smashPositionStart.y - smashTravelDistanceY * (t / smashDownDuration), transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = smashPositionStart + Vector3.down * smashTravelDistanceY;
        if(OnHandSmashDown != null)
        {
            OnHandSmashDown(transform.position);
        }
        // Reset hand position
        StartCoroutine(ResetAfterSmash(resetTime));
        GameManager.Instance.GetSmashParticle(transform.position + Vector3.down * 0.3f);
        StartCoroutine(VibrateController(smashGamePadRumbleDuration));
        camShake.shakeAmount = smashCamShakeAmount;
        camShake.shakeDuration = smashCamShakeDuration;
        yield return new WaitForSeconds(0.2f);
        canKill = false;
    }

    protected virtual void SpecialSmash()
    {
        isInSpecialSmash = true;
        rBody.constraints = RigidbodyConstraints.FreezeRotation;
        StartCoroutine(SpecialSmashDown());
    }

    IEnumerator SpecialSmashDown()
    {
        for (int i = 0; i < timesHittingWithSpecialSmash; i++)
        {
            canKill = true;
            smashPositionStart = transform.position;
            for (float t = 0f; t < smashDownDuration; t += Time.deltaTime)
            {
                transform.position = new Vector3(transform.position.x, smashPositionStart.y - smashTravelDistanceY * (t / smashDownDuration), transform.position.z);
                yield return new WaitForEndOfFrame();
            }
            transform.position = new Vector3(transform.position.x, smashPositionStart.y - smashTravelDistanceY, transform.position.z);
            if (OnHandSmashDown != null)
            {
                OnHandSmashDown(transform.position);
            }
            GameManager.Instance.GetSmashParticle(transform.position + Vector3.down * 0.3f);
            StartCoroutine(VibrateController(smashGamePadRumbleDuration));
            camShake.shakeAmount = smashCamShakeAmount;
            camShake.shakeDuration = smashCamShakeDuration;
            yield return new WaitForSeconds(resetTime * 0.1f);
            canKill = false;
            yield return new WaitForSeconds(resetTime * 0.1f);
            for (float t = 0f; t < resetTransformDuration * 0.3f; t += Time.deltaTime)
            {
                transform.position = new Vector3(transform.position.x, (smashPositionStart.y - smashTravelDistanceY) + smashTravelDistanceY * (t / (resetTransformDuration * 0.3f)), transform.position.z);
                yield return new WaitForEndOfFrame();
            }
            transform.position = new Vector3(transform.position.x, smashPositionStart.y, transform.position.z);
            rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        canSmash = false;
        canKill = true;
        rBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        smashPositionStart = transform.position;
        for (float t = 0f; t < smashDownDuration; t += Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x, smashPositionStart.y - smashTravelDistanceY * (t / smashDownDuration), transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = smashPositionStart + Vector3.down * smashTravelDistanceY;
        if (OnHandSmashDown != null)
        {
            OnHandSmashDown(transform.position);
        }
        GameManager.Instance.GetSmashParticle(transform.position + Vector3.down * 0.3f);
        StartCoroutine(VibrateController(smashGamePadRumbleDuration));
        camShake.shakeAmount = smashCamShakeAmount;
        camShake.shakeDuration = smashCamShakeDuration;
        // Reset hand position
        StartCoroutine(ResetAfterSmash(resetTime));
        if(OnSpecialSmashEnd != null)
        {
            OnSpecialSmashEnd();
        }
        isInSpecialSmash = false;
        yield return new WaitForSeconds(0.1f);
        canKill = false;
    }

    IEnumerator VibrateController(float duration)
    {
        GamePad.SetVibration(PlayerIndex.One, padState.Triggers.Left, padState.Triggers.Right);
        yield return new WaitForSeconds(duration);
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
    }

    IEnumerator ResetAfterSmash(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        for (float t = 0f; t < resetTransformDuration; t += Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x, (smashPositionStart.y - smashTravelDistanceY) + smashTravelDistanceY * (t / resetTransformDuration), transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = smashPositionStart;
        rBody.constraints = RigidbodyConstraints.FreezeRotation;

        canSmash = true;
    }

}
