using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform capsule;
    private float xRotation = 0f;
    private float yRotation = 0f;

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
    RoverMove roverMove;

    // Start is called before the first frame update
    void Start()
    {
        roverMove = FindObjectOfType<RoverMove>();
        transform.rotation = roverMove.transform.rotation;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("f"))
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            capsule.rotation = Quaternion.Euler(0, yRotation, 0);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

}
