using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public Transform gunTip, cam;
    public LayerMask grappleable;
    [HideInInspector]
    public bool isGrappling;
    private float maxDistance = 30f;
    private PlayerBehavior player;
    private Rigidbody rigidBod;
    private SpringJoint joint;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        player = FindObjectOfType<PlayerBehavior>();
    }

    private void Update()
    {
        
        if (!player.CharacterControl.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            player.CharacterControl.enabled = false;
            rigidBod = player.GetComponent<Rigidbody>();
            StartGrapple();
        }
        else if(isGrappling && Input.GetKeyDown(KeyCode.Space))
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        if(isGrappling)
        {
            DrawRope();
        }   
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(player.transform.position, cam.forward, out hit, maxDistance))
        {
            isGrappling = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint);

            //Distance grapple will try to keep from grapple point
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    private void DrawRope()
    {
        if (!joint)
        {
            return;
        }
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);

    }

    private void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
        
    }

    /* IEnumerator Grappled()
    {
        
    } */


}
