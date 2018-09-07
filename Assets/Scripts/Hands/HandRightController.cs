using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRightController : BaseHandController
{
    private void Update()
    {
        //movement and controller stick deadzone
        moveInput = new Vector3(Input.GetAxis("Horizontal2"), 0f, Input.GetAxis("Vertical2"));

        //moveInput = moveInput.normalized * ((moveInput.magnitude - deadzone) / (1 - deadzone));

        moveVelocity = moveInput * moveSpeed;
        rBody.velocity = moveVelocity;

        //smash ground using trigger
        triggerInput = new Vector3(0f, Input.GetAxis("TriggerRight"), 0f);

        if (triggerInput.magnitude > 0.2 && canSmash == true)
        {
            smash();
        }
    }
}