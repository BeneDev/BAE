﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class HandRightController : BaseHandController
{
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
    }
}