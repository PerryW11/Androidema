using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float CameraMoveSpeed = 120f;
    public GameObject CameraFollowObj;
    public GameObject CameraZoomFollowObj;
    public float clampAngle = 80f;
    public float inputSensitivity = 350f;
    public float mouseX;
    public float mouseY;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    Quaternion localRotation;

    private void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        rotY += mouseX * inputSensitivity * Time.deltaTime;
        rotX -= mouseY * inputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        localRotation = Quaternion.Euler(rotX, rotY, 0); 

        transform.rotation = localRotation;
    }

    private void LateUpdate()
    {
        CameraUpdater();
    }

    private void CameraUpdater()
    {
        Transform target;
        if (!Input.GetMouseButton(1))
        {
            target = CameraFollowObj.transform;
        }
        else
        {
            localRotation = Quaternion.Euler(0, rotY, 0); // use rotX for x if verticality is involved
            transform.rotation = localRotation;
            target = CameraZoomFollowObj.transform;
        }

        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
