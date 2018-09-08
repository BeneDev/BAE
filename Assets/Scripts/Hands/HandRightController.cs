using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class HandRightController : BaseHandController
{
    HandLeftController handLeft;

    protected override void Awake()
    {
        base.Awake();
        handLeft = GameObject.FindGameObjectWithTag("HandLeft").GetComponent<HandLeftController>();
    }

    protected override void Update()
    {
        //movement and controller stick deadzone
        moveInput = new Vector3(Input.GetAxis("Horizontal2"), 0f, Input.GetAxis("Vertical2"));

        //moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1 - deadzone));

        moveVelocity = moveInput * moveSpeed;
        rBody.velocity = moveVelocity;

        //smash ground using trigger
        triggerInput = new Vector3(0f, Input.GetAxis("TriggerRight"), 0f);
        base.Update();
        if (isLeftStickDown && isRightStickDown && canSmash && handLeft.CanSmash && weakSpot.RageMeter == weakSpot.MaxRage)
        {
            SpecialSmash();
            isLeftStickDown = false;
            isRightStickDown = false;
        }
        toOtherHand = handLeft.transform.position - transform.position;
        if (toOtherHand.magnitude < distanceStartFistRotating && toOtherHand.magnitude > distanceFistRotationComplete)
        {
            RotateForFistBump();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "HandLeft")
        {
            transform.rotation = Quaternion.Euler(fistBumpRotation);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "HandLeft")
        {
            //transform.rotation = normalRot;
        }
    }
}