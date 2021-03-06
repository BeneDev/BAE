﻿using System.Collections;
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

    public float ResetTime
    {
        get
        {
            return resetTime;
        }
        set
        {
            resetTime = value;
        }
    }

    public GameObject CurrentPowerup
    {
        get
        {
            return currentPowerup;
        }
        set
        {
            Destroy(currentPowerup);
            currentPowerup = value;
        }
    }

    public float Speed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
        }
    }

    public event System.Action<Vector3> OnHandSmashDown;
    public event System.Action OnSpecialSmashEnd;
    public event System.Action OnSpecialSmashStarted;
    public event System.Action<Sprite> OnPowerupChanged;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float smashSpeed = 2f;
    [SerializeField] protected float resetDistance = 0.01f;
    [SerializeField] protected float resetTime = 2f;
    [SerializeField] protected float resetSpeed = 1f;
    protected float smashTravelDistanceY;
    protected Vector3 normalPos;
    protected const float normalSmashTravelDistanceY = 2.5f;
    [SerializeField] protected float smashDownDuration = 0.2f;
    [SerializeField] protected float smashCamShakeAmount = 0.3f;
    [SerializeField] protected float smashCamShakeDuration = 0.15f;
    [SerializeField] protected float smashGamePadRumbleDuration = 0.2f;
    [SerializeField] protected float resetTransformDuration = 0.2f;

    [SerializeField] protected int timesHittingWithSpecialSmash = 10;
    [SerializeField] protected float timeBeforeSpecialSmash = 0f;
    
    protected float t1;
    protected float t2;
    protected float startTime;
    protected float smashLength;

    protected Rigidbody rBody;

    protected Quaternion normalRot;
    [SerializeField] protected Vector3 fistBumpRotation;
    protected Vector3 toOtherHand;
    [SerializeField] protected float distanceStartFistRotating = 5f;

    protected bool canSmash = true;
    protected bool isInSpecialSmash = false;

    protected Vector3 moveInput;
    protected Vector3 smashPositionStart;
    protected Vector3 moveVelocity;

    protected float triggerInput;

    protected bool canKill = false;

    protected CameraShake camShake;

    protected GamePadState padState;

    protected bool isLeftStickDown = false;
    protected bool isRightStickDown = false;

    protected WeakSpotController weakSpot;

    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip[] aClips;
    [Range(0.5f, 2f), SerializeField] float minPitch;
    [Range(1f, 3f), SerializeField] float maxPitch;

    [SerializeField] int holdStickDownInputForFrames = 5;

    protected Animator anim;

    GameObject currentPowerup;
    [SerializeField] protected Sprite defaultPowerupSprite;

    protected virtual void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        camShake = Camera.main.GetComponent<CameraShake>();
        weakSpot = GameObject.FindGameObjectWithTag("WeakSpot").GetComponent<WeakSpotController>();
        normalRot = transform.rotation;
        normalPos = transform.position;
        anim = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if(!GameManager.Instance.IsPSInput)
        {
            if (triggerInput != 0f && canSmash && !isInSpecialSmash && !anim.enabled)
            {
                Smash();
                padState = GamePad.GetState(PlayerIndex.One);
            }
        }
        else
        {
            if (triggerInput > 0f && canSmash && !isInSpecialSmash && !anim.enabled)
            {
                Smash();
                padState = GamePad.GetState(PlayerIndex.One);
            }
        }
        
        if(!GameManager.Instance.IsPSInput && Input.GetButtonDown("LeftStickDown"))
        {
            isLeftStickDown = true;
            CancelInvoke("ResetLeftStick");
            Invoke("ResetLeftStick", holdStickDownInputForFrames * Time.deltaTime);
        }
        else if(GameManager.Instance.IsPSInput && Input.GetButtonDown("PSLeftStickDown"))
        {
            isLeftStickDown = true;
            CancelInvoke("ResetLeftStick");
            Invoke("ResetLeftStick", holdStickDownInputForFrames * Time.deltaTime);
        }
        if(!GameManager.Instance.IsPSInput && Input.GetButtonDown("RightStickDown"))
        {
            isRightStickDown = true;
            CancelInvoke("ResetRightStick");
            Invoke("ResetRightStick", holdStickDownInputForFrames * Time.deltaTime);
        }
        else if (GameManager.Instance.IsPSInput && Input.GetButtonDown("PSRightStickDown"))
        {
            isRightStickDown = true;
            CancelInvoke("ResetRightStick");
            Invoke("ResetRightStick", holdStickDownInputForFrames * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        rBody.velocity = Vector3.zero;
        rBody.useGravity = true;
    }

    private void OnEnable()
    {
        rBody.useGravity = false;
    }

    void ResetRightStick()
    {
        isRightStickDown = false;
    }

    void ResetLeftStick()
    {
        isLeftStickDown = false;
    }

    public void DisableAnimator()
    {
        anim.enabled = false;
    }

    //protected void RotateForFistBump()
    //{
    //    Quaternion newRot = Quaternion.LookRotation(toOtherHand, Vector3.up);
    //    //newRot.Pow(toOtherHand.magnitude / distanceStartFistRotating);
    //    newRot = Quaternion.Euler(new Vector3(0f, newRot.eulerAngles.y, 0f));
    //    transform.rotation = newRot;
    //}

    //protected IEnumerator ResetRotation(float seconds)
    //{
    //    Quaternion startRot = transform.rotation;
    //    Quaternion rotDiff = startRot * Quaternion.Inverse(normalRot);
    //    for (float t = 0; t < seconds; t += Time.deltaTime)
    //    {
    //        transform.rotation = rotDiff.Pow((t / seconds)) * startRot;
    //        yield return new WaitForEndOfFrame();
    //    }
    //    transform.rotation = normalRot;
    //    yield break;
    //}

    //smash ground
    protected virtual void Smash()
    {
        canSmash = false;
        rBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        smashTravelDistanceY = transform.position.y - 0.5f;
        StartCoroutine(SmashDown());
    }

    public IEnumerator ToInitialPos()
    {
        Vector3 startPos = transform.position;
        for (float t = 0f; t < 0.25f; t += Time.deltaTime)
        {
            transform.position = startPos + ((normalPos - startPos) * (t / 0.25f));
            yield return new WaitForEndOfFrame();
        }
        transform.position = normalPos;
    }

    IEnumerator SmashDown()
    {
        aSource.PlayOneShot(aClips[0], 0.1f);
        canKill = true;
        smashPositionStart = transform.position;
        for (float t = 0f; t < smashDownDuration; t += Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x, smashPositionStart.y - smashTravelDistanceY * (t / smashDownDuration), transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        smashPositionStart.y = normalPos.y;
        transform.position = smashPositionStart + Vector3.down * smashTravelDistanceY;
        if(OnHandSmashDown != null)
        {
            StartCoroutine(PlayAtRandomPitch(aClips[1]));
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

    public void ChangePowerup(Sprite icon = null)
    {
        if(OnPowerupChanged != null)
        {
            if(icon)
            {
                OnPowerupChanged(icon);
            }
            else
            {
                OnPowerupChanged(defaultPowerupSprite);
            }
        }
    }

    IEnumerator PlayAtRandomPitch(AudioClip clip)
    {
        aSource.pitch = Random.Range(minPitch, maxPitch);
        aSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        aSource.pitch = 1f;
    }

    protected virtual void SpecialSmash()
    {
        isInSpecialSmash = true;
        rBody.constraints = RigidbodyConstraints.FreezeRotation;
        StartCoroutine(SpecialSmashDown());
    }

    IEnumerator SpecialSmashDown()
    {
        yield return new WaitForSeconds(timeBeforeSpecialSmash);
        if (OnSpecialSmashStarted != null)
        {
            OnSpecialSmashStarted();
        }
        for (int i = 0; i < timesHittingWithSpecialSmash; i++)
        {
            canKill = true;
            smashTravelDistanceY = transform.position.y - 0.5f;
            smashPositionStart = transform.position;
            for (float t = 0f; t < smashDownDuration; t += Time.deltaTime)
            {
                transform.position = new Vector3(transform.position.x, smashPositionStart.y - smashTravelDistanceY * (t / smashDownDuration), transform.position.z);
                yield return new WaitForEndOfFrame();
            }
            smashPositionStart.y = normalPos.y;
            transform.position = new Vector3(transform.position.x, smashPositionStart.y - smashTravelDistanceY, transform.position.z);
            if (OnHandSmashDown != null)
            {
                OnHandSmashDown(transform.position);
                StartCoroutine(PlayAtRandomPitch(aClips[1]));
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
                transform.position = new Vector3(transform.position.x, (smashPositionStart.y - normalSmashTravelDistanceY) + normalSmashTravelDistanceY * (t / (resetTransformDuration * 0.3f)), transform.position.z);
                yield return new WaitForEndOfFrame();
            }
            transform.position = new Vector3(transform.position.x, smashPositionStart.y, transform.position.z);
            rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        canSmash = false;
        canKill = true;
        rBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        smashPositionStart = transform.position;
        smashTravelDistanceY = transform.position.y - 0.5f;
        for (float t = 0f; t < smashDownDuration; t += Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x, smashPositionStart.y - smashTravelDistanceY * (t / smashDownDuration), transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        smashPositionStart.y = normalPos.y;
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
        aSource.PlayOneShot(aClips[2], 0.1f);
        for (float t = 0f; t < resetTransformDuration; t += Time.deltaTime)
        {
            transform.position = new Vector3(transform.position.x, (smashPositionStart.y - normalSmashTravelDistanceY) + normalSmashTravelDistanceY * (t / resetTransformDuration), transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = smashPositionStart;
        rBody.constraints = RigidbodyConstraints.FreezeRotation;

        canSmash = true;
    }

}
