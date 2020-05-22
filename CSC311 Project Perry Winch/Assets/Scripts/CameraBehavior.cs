using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform target;
   
    [System.Serializable]
    public class PositionSettings
    {
        public Vector3 targetPosOffset = new Vector3(0, 0.55f, 0);
        public float lookSmooth = 100f;
        public float distanceFromTarget = -3f;
        public float zoomSmooth = 100f;
        public float maxZoom = -0.5f;
        public float minZoom = -6f;
        public bool smoothFollow = true;
        public float smooth = 0.05f;

        [HideInInspector]
        public float newDistance = -3f;
        [HideInInspector]
        public float adjustmentDistance = -3f;
    }

    [System.Serializable]
    public class OrbitSettings
    {
        public float xRotation = -20f;
        public float yRotation = -180f;
        public float maxXRotation = 25f;
        public float minXRotation = -85f;
        public float vOrbitSmooth = 150f;
        public float hOrbitSmooth = 150f;
        public float orbitSpeed = 5f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
        public string ORBIT_HORIZONTAL = "OrbitHorizontal";
        public string ORBIT_VERTICAL = "OrbitVertical";
        public string ZOOM = "Mouse ScrollWheel";
    }

    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAdjustedCollisionLines = true;
    }

    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();
    public DebugSettings debug = new DebugSettings();
    public CollisionHandler collision = new CollisionHandler();

    private Vector3 targetPos = Vector3.zero;
    private Vector3 destination = Vector3.zero;
    private Vector3 adjustedDestination = Vector3.zero;
    private Vector3 camSpeed = Vector3.zero;
    private PlayerBehavior player;
    private float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetCameraTarget(target);

        player = FindObjectOfType<PlayerBehavior>();

        MoveToTarget();

        collision.Initialize(Camera.main);
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);
    }

    public void SetCameraTarget(Transform t)
    {
        target = t;

        if(target != null)
        {
          
        }
        else
        {
            Debug.LogError("Camera needs a target");
        }
    }

   private void GetInput()
    {
        vOrbitInput = Input.GetAxis("Mouse Y");
        hOrbitInput = Input.GetAxis("Mouse X");
        hOrbitSnapInput = Input.GetAxisRaw(input.ORBIT_HORIZONTAL_SNAP);
        zoomInput = Input.GetAxisRaw(input.ZOOM);
    }

    private void Update()
    {
        GetInput();
        OrbitTarget();
        ZoomInOnTarget();
    }

    private void FixedUpdate()
    {
        MoveToTarget();
        LookAtTarget();
        OrbitTarget();
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.adjustedCameraClipPoints);
        collision.UpdateCameraClipPoints(destination, transform.rotation, ref collision.desiredCameraClipPoints);

        //Draw debug lines
        for (int i = 0; i < 5; i++)
        {
            if(debug.drawDesiredCollisionLines)
            {
                Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.white);
            }
            if(debug.drawAdjustedCollisionLines)
            {
                Debug.DrawLine(targetPos, collision.adjustedCameraClipPoints[i], Color.green);
            }
        }

        collision.CheckColliding(targetPos); //Using raycasts here
        position.adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(targetPos);

    }

    private void MoveToTarget()
    {
        targetPos = target.position + Vector3.up * position.targetPosOffset.y + Vector3.forward * position.targetPosOffset.z + transform.TransformDirection(Vector3.right * position.targetPosOffset.x);
        destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * position.distanceFromTarget;
        destination += targetPos;
        transform.position = destination;

        if (collision.colliding)
        {
            adjustedDestination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * Vector3.forward * position.adjustmentDistance;
            adjustedDestination += targetPos;

            if (position.smoothFollow)
            {
                transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref camSpeed, position.smooth);
            }
            else
            {
                transform.position = adjustedDestination;
            }
        }
        else
        {
            if (position.smoothFollow)
            {
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref camSpeed, position.smooth);
            }
            else
            {
                transform.position = destination;
            }
        }
    }

    private void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, position.lookSmooth * Time.deltaTime);
    }

    private void OrbitTarget()
    {
        if(hOrbitSnapInput > 0)
        {
            orbit.yRotation = -180f;
        }

        orbit.xRotation += vOrbitInput * orbit.vOrbitSmooth * Time.deltaTime * orbit.orbitSpeed;
        orbit.yRotation += hOrbitInput * orbit.hOrbitSmooth * Time.deltaTime * orbit.orbitSpeed;

        if (orbit.xRotation > orbit.maxXRotation)
        {
            orbit.xRotation = orbit.maxXRotation;
        }
        if(orbit.xRotation < orbit.minXRotation)
        {
            orbit.xRotation = orbit.minXRotation;
        }
    }

    private void ZoomInOnTarget()
    {
        position.distanceFromTarget += zoomInput * position.zoomSmooth * Time.deltaTime;
        
        if(position.distanceFromTarget > position.maxZoom)
        {
            position.distanceFromTarget = position.maxZoom;
        }
        if(position.distanceFromTarget < position.minZoom)
        {
            position.distanceFromTarget = position.minZoom;
        }
    }


    //This class is to handle camera collision and occlusion
    [System.Serializable]
    public class CollisionHandler
    {
        public LayerMask collisionLayer;

        [HideInInspector]
        public bool colliding = false;
        [HideInInspector]
        public Vector3[] adjustedCameraClipPoints;
        [HideInInspector]
        public Vector3[] desiredCameraClipPoints;

        private Camera camera;

        public void Initialize(Camera cam)
        {
            camera = cam;
            // 4 points on near clip plane and 1 point for the camera's position
            adjustedCameraClipPoints = new Vector3[5];
            desiredCameraClipPoints = new Vector3[5];
        }

        public void UpdateCameraClipPoints(Vector3 camPosition, Quaternion atRotation, ref Vector3[] intoArray)
        {
            if (!camera)
            {
                return;
            }

            //Clear the contents of the array
            intoArray = new Vector3[5];

            float z = camera.nearClipPlane;
            float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
            float y = x / camera.aspect;

            //Top left
            intoArray[0] = (atRotation * new Vector3(-x, y, z)) + camPosition; // Added and rotated point relative to the camera

            //Top right
            intoArray[1] = (atRotation * new Vector3(x, y, z)) + camPosition;

            //Bottom left
            intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + camPosition;

            //Bottom right
            intoArray[3] = (atRotation * new Vector3(x, -y, z)) + camPosition;

            //Camera's position
            intoArray[4] = camPosition - camera.transform.forward;
        }

        private bool CollisionDetectedAtClipPoints (Vector3[] clipPoints, Vector3 fromPosition)
        {
            for (int i = 0; i < clipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
                float distance = Vector3.Distance(clipPoints[i], fromPosition);
                if(Physics.Raycast(ray, distance, collisionLayer))
                {
                    return true;
                }
            }

            return false;
        }


        public float GetAdjustedDistanceWithRayFrom(Vector3 from)
        {
            float distance = -1f;
            
            for(int i = 0; i < desiredCameraClipPoints.Length; i++)
            {
                Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    if (distance == -1f)
                    {
                        distance = hit.distance;
                    }
                    else
                    {
                        if(hit.distance < distance)
                        {
                            distance = hit.distance;
                        }
                    }
                }
            }

            if (distance == 1)
            {
                return 0;
            }
            else
            {
                return distance;
            }
        }

        public void CheckColliding(Vector3 targetPosition)
        {
            if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
            {
                colliding = true;
            }
            else
            {
                colliding = false;
            }
        }

    }

}
