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
        if(!GameManager.Instance.IsPSInput)
        {
            moveInput = new Vector3(Input.GetAxis("Horizontal2"), 0f, Input.GetAxis("Vertical2"));
            //smash ground using trigger
            triggerInput = Input.GetAxis("TriggerRight");
        }
        else
        {
            moveInput = new Vector3(Input.GetAxis("PSHorizontal2"), 0f, Input.GetAxis("PSVertical2"));
            //smash ground using trigger
            triggerInput = Input.GetAxis("PSTriggerRight");
        }

        //moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1 - deadzone));

        moveVelocity = moveInput * moveSpeed;
        rBody.velocity = moveVelocity;

        base.Update();
        if (isLeftStickDown && isRightStickDown && canSmash && handLeft.CanSmash && weakSpot.RageMeter == weakSpot.MaxRage)
        {
            SpecialSmash();
            isLeftStickDown = false;
            isRightStickDown = false;
        }
        toOtherHand = handLeft.transform.position - transform.position;
    }

    // FOR FISTBUMPING
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "HandLeft")
    //    {
    //        Quaternion newRot = Quaternion.LookRotation(toOtherHand, Vector3.up);
    //        newRot = Quaternion.Euler(new Vector3(0f, newRot.eulerAngles.y, 0f));
    //        transform.rotation = newRot;
    //    }
    //}
}