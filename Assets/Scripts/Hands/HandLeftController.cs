using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class HandLeftController : BaseHandController
{
    private void Update()
    {
        //movement and controller stick deadzone
        moveInput = new Vector3(Input.GetAxis("Horizontal1"), 0f, Input.GetAxis("Vertical1"));

        //moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1 - deadzone));

        moveVelocity = moveInput * moveSpeed;
        rBody.velocity = moveVelocity;

        //smash ground using trigger
        triggerInput = new Vector3(0f, Input.GetAxis("TriggerLeft"), 0f);

        if(triggerInput.magnitude > 0.2 && canSmash == true)
        {
            smash();
            padState = GamePad.GetState(PlayerIndex.One);
        }
    }
}