using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent
{
    public void Move(CharacterController characterController, Vector3 direction, Vector3 speed)
    {
        Vector3 move = new Vector3(direction.x * speed.x, direction.y * speed.y, direction.z * speed.z);
        characterController.Move(move * Time.deltaTime);
    }
}
