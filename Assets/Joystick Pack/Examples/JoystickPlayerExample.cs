using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    [SerializeField] public GameObject cam;

    public bool end = false;
    public float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;
    public Vector3 direction;

    public void FixedUpdate()
    {
        direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        //rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);

        if(!end)
        {
            cam.transform.position = new Vector3(transform.position.x, cam.transform.position.y, transform.position.z);
        }
    }
}