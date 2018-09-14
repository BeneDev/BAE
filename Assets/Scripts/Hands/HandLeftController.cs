using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class HandLeftController : BaseHandController
{
    HandRightController handRight;

    protected override void Awake()
    {
        base.Awake();
        handRight = GameObject.FindGameObjectWithTag("HandRight").GetComponent<HandRightController>();
    }

    protected override void Update()
    {
        if (GameManager.Instance.IsPaused)
        {
            rBody.velocity = Vector3.zero;
            return;
        }
        //movement and controller stick deadzone
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        //moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1 - deadzone));

        moveVelocity = moveInput * moveSpeed;
        rBody.velocity = moveVelocity;

        if(!GameManager.Instance.IsPSInput)
        {
            //smash ground using trigger
            triggerInput = Input.GetAxis("TriggerLeft");
        }
        else
        {
            //smash ground using trigger
            triggerInput = Input.GetAxis("PSTriggerLeft");
        }
        base.Update();
        if (isLeftStickDown && isRightStickDown && canSmash && handRight.CanSmash && weakSpot.RageMeter >= weakSpot.MaxRage && !anim.enabled)
        {
            SpecialSmash();
            isLeftStickDown = false;
            isRightStickDown = false;
        }
        toOtherHand = handRight.transform.position - transform.position;
    }

    // FOR FISTBUMPING
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "HandRight")
        {
            //Quaternion newRot = Quaternion.LookRotation(toOtherHand, Vector3.up);
            //newRot = Quaternion.Euler(new Vector3(0f, newRot.eulerAngles.y, 0f));
            //transform.rotation = newRot;
            camShake.shakeAmount = smashCamShakeAmount * 0.5f;
            camShake.shakeDuration = smashCamShakeDuration * 0.5f;
        }
    }
}