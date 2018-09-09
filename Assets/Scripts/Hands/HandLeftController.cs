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
        //movement and controller stick deadzone
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        //moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1 - deadzone));

        moveVelocity = moveInput * moveSpeed;
        rBody.velocity = moveVelocity;

        //smash ground using trigger
        triggerInput = new Vector3(0f, Input.GetAxis("TriggerLeft"), 0f);
        base.Update();
        if (isLeftStickDown && isRightStickDown && canSmash && handRight.CanSmash && weakSpot.RageMeter >= weakSpot.MaxRage)
        {
            SpecialSmash();
            isLeftStickDown = false;
            isRightStickDown = false;
        }
        toOtherHand = handRight.transform.position - transform.position;
    }

    // FOR FISTBUMPING
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag == "HandRight")
    //    {
    //        Quaternion newRot = Quaternion.LookRotation(toOtherHand, Vector3.up);
    //        newRot = Quaternion.Euler(new Vector3(0f, newRot.eulerAngles.y, 0f));
    //        transform.rotation = newRot;
    //        camShake.shakeAmount = smashCamShakeAmount * 0.5f;
    //        camShake.shakeDuration = smashCamShakeDuration * 0.5f;
    //    }
    //}
}