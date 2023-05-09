using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDeleteMoveScript : MonoBehaviour
{
    public float speed = 5.0f;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 position = transform.position;
        position.x += horizontalInput * speed * Time.deltaTime;
        position.z += verticalInput * speed * Time.deltaTime;
        transform.position = position;
    }

}