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
            transform.rotation = Quaternion.Euler(eulerRotation);
        }

        DetectMovement();
       // DetectRotation();
        DetectFire();
        
    }

    

    private void DetectMovement()
    {

        //charAnim.SetBool("IsRunning", vInput != 0);
        //charAnim.SetBool("IsJumping", Input.GetKeyDown(KeyCode.Space) && charCon.isGrounded);

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

   /* private void DetectRotation()
    {
        if (hInput != 0)
        {
            Vector3 rotation = Vector3.up * hInput;
            transform.Rotate(rotation * Time.deltaTime);
        }
    }*/

    private void DetectFire()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            audWeaponFire.Play();
            GameObject newBullet = Instantiate(bullet, this.transform.position + (this.transform.forward * 1.2f), this.transform.rotation) as GameObject;
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
