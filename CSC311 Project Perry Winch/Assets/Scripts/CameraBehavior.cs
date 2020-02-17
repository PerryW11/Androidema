using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Vector3 camOffset = new Vector3(0, 1.5f, -3.2f);

    private Transform target;

    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    void LateUpdate()
    {
        this.transform.position = target.TransformPoint(camOffset);
     
        this.transform.LookAt(target);

        this.transform.position = target.TransformPoint(0.7f, 1.9f, -3.0f); //Over the shoulder perspective
    }
}
