using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Vector3 camOffset = new Vector3(0, 0, 0);

    private Transform target;

    private Transform target2;

    void Start()
    {
        //target = GameObject.Find("Player").transform;
        target = GameObject.Find("CameraLocation").transform;
        target2 = GameObject.Find("CameraLookLocation").transform;
    }

    void LateUpdate()
    {
        this.transform.position = target.position;
        this.transform.LookAt(target2);
    }
}
