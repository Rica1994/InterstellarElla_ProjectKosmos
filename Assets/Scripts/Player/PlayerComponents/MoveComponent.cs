using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent
{
    // used in speederGround
    public void Move(CharacterController characterController, Vector3 direction, Vector3 speed)
    {
        Vector3 move = new Vector3(direction.x * speed.x, direction.y * speed.y, direction.z * speed.z);

        characterController.Move(move * Time.deltaTime);
    }

    // used in exploring
    public void Move(CharacterController characterController, Vector2 direction, float speed)
    {
        Vector3 move = new Vector3(direction.x * speed, 0, direction.y * speed);

        characterController.Move(move * Time.deltaTime);
    }

    // used in SpeederSpace
    public void Move(CharacterController characterController, Vector3 direction, float speed)
    {
        Move(characterController, direction, new Vector3(speed, speed, speed));
    }
}
