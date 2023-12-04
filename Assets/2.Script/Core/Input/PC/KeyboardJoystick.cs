using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardJoystick : PlayerJoystick
{
    public override void SetDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveDirection.x = h;
        moveDirection.z = v;
    }
}
