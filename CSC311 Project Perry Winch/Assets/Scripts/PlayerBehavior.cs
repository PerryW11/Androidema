using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBehavior : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 150f;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;
    private float gravity = 8f;

    public GameObject bullet;
    public float bulletSpeed = 60f;

    private float vInput;
    private float hInput;
    private Vector3 moveDir;
    private Quaternion targetRotation;

    public float timeInvincible = 2f;
    public Animator charAnim;

    public AudioSource audWeaponFire;

    //private Rigidbody _rb;
    private CapsuleCollider col;
    private CharacterController charCon;
    public GameBehavior gameManager;
    public CameraFollow cam;

    public GameObject spine;


    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    public CharacterController CharacterControl
    {
        get { return charCon; }
    }

    private void Start()
    {
       // _rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        charCon = GetComponent<CharacterController>();
        targetRotation = transform.rotation;
        gameManager = FindObjectOfType<GameBehavior>();
       // gameManager = GameObject.Find("Game Manager").GetComponent<GameBehavior>();
    }


    private void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * moveSpeed;
        //targetRotation *= Quaternion.AngleAxis(hInput * Time.deltaTime, Vector3.up);
        
        if(Input.GetMouseButton(1) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            Vector3 eulerRotation = new Vector3(transform.eulerAngles.x, cam.transform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(eulerRotation), Time.deltaTime * 8f);
            //charAnim.enabled = false;
            //spine.transform.rotation = cam.transform.rotation;
            
        }

        DetectMovement();
       // DetectRotation();
        DetectFire();
        
    }

    

    private void DetectMovement()
    {

        //Animations for player movement
        charAnim.SetBool("IsWalkingForward", Mathf.Sign(vInput) == Mathf.Sign(1) && vInput != 0);
        charAnim.SetBool("IsWalkingForwardLeft", Mathf.Sign(vInput) == Mathf.Sign(1) && Mathf.Sign(hInput) == Mathf.Sign(-1) && vInput != 0);
        charAnim.SetBool("IsWalkingForwardRight", Mathf.Sign(vInput) == Mathf.Sign(1) && Mathf.Sign(hInput) == Mathf.Sign(1) && vInput != 0 && hInput != 0);
        charAnim.SetBool("IsWalkingBackward", Mathf.Sign(vInput) == Mathf.Sign(-1));
        charAnim.SetBool("IsWalkingBackwardLeft", Mathf.Sign(vInput) == Mathf.Sign(-1) && Mathf.Sign(hInput) == Mathf.Sign(-1));
        charAnim.SetBool("IsWalkingBackwardRight", Mathf.Sign(vInput) == Mathf.Sign(-1) && Mathf.Sign(hInput) == Mathf.Sign(1) && hInput != 0);
        charAnim.SetBool("IsWalkingSidewaysLeft", Mathf.Sign(hInput) == Mathf.Sign(-1) && vInput == 0);
        charAnim.SetBool("IsWalkingSidewaysRight", Mathf.Sign(hInput) == Mathf.Sign(1) && vInput == 0 && hInput != 0);



        if (charCon.isGrounded)
        {
            moveDir = new Vector3(hInput, 0, vInput);
            moveDir = transform.TransformDirection(moveDir);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDir.y = gameManager.jumpVelocity;
            }
            else
            {
                moveDir.y = -0.5f;
            }
        }
        else
        {
            moveDir = new Vector3(hInput, moveDir.y, vInput);
            moveDir = transform.TransformDirection(moveDir);
            moveDir.y -= gravity * Time.deltaTime;
        }
        charCon.Move(moveDir * Time.deltaTime);
    }

    private void DetectFire()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !gameManager.GamePaused)
        {
            audWeaponFire.Play();
                                                                            // Slight in front of player - Slightly up to gun level - In front of gun
            GameObject newBullet = Instantiate(bullet, transform.position + (transform.forward * 1.2f) + (transform.up * 0.5f) + (transform.right * 0.25f), this.transform.rotation) as GameObject;
            Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
            bulletRB.velocity = this.transform.forward * bulletSpeed;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100);
            int i = 0;
            while(i < hitColliders.Length)
            {
                Collider col = hitColliders[i];
                if(col.gameObject.tag == "Enemy")
                {
                    col.GetComponent<EnemyBehavior>().HandlePlayerSight();
                }
                i++;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!gameManager.playerInvincible)
        {
            StartCoroutine(TempInvincibility());
        }
    }

    public void CallTempInvincibility()
    {
        StartCoroutine(TempInvincibility());
    }

    IEnumerator TempInvincibility()
    {
        gameManager.playerInvincible = true;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        float timeStart = Time.time;
        while(Time.time - timeStart < timeInvincible)
        {
            mr.enabled = !mr.enabled;
            yield return new WaitForSeconds(0.2f);
            mr.enabled = !mr.enabled;
            yield return new WaitForSeconds(0.08f);
        }
        mr.enabled = false;
        gameManager.playerInvincible = false;
    }
}
