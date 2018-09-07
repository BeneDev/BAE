﻿using System.Collections;
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
        moveInput = new Vector3(Input.GetAxis("Horizontal1"), 0f, Input.GetAxis("Vertical1"));

        //moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1 - deadzone));

        moveVelocity = moveInput * moveSpeed;
        rBody.velocity = moveVelocity;

        //smash ground using trigger
        triggerInput = new Vector3(0f, Input.GetAxis("TriggerLeft"), 0f);
        base.Update();
        if (isLeftStickDown && isRightStickDown && canSmash && handRight.CanSmash)
        {
            SpecialSmash();
            isLeftStickDown = false;
            isRightStickDown = false;
        }
    }
}