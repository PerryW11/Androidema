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
    private CapsuleCollider _col;
    private CharacterController charCon;
    public GameBehavior _gameManager;

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
        _col = GetComponent<CapsuleCollider>();
        charCon = GetComponent<CharacterController>();
        targetRotation = transform.rotation;
       // _gameManager = GameObject.Find("Game Manager").GetComponent<GameBehavior>();
    }


    private void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * rotateSpeed;
        targetRotation *= Quaternion.AngleAxis(hInput * Time.deltaTime, Vector3.up);
        transform.rotation = targetRotation;

        DetectMovement();
        DetectRotation();
        DetectFire();
        
    }

    private void DetectMovement()
    {

        charAnim.SetBool("IsRunning", vInput != 0);
        charAnim.SetBool("IsJumping", Input.GetKeyDown(KeyCode.Space) && charCon.isGrounded);

        if (charCon.isGrounded)
        {
            moveDir = new Vector3(0, 0, vInput);
            moveDir = transform.TransformDirection(moveDir);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDir.y = _gameManager.jumpVelocity;
            }
            else
            {
                moveDir.y = -0.5f;
            }
        }
        else
        {
            //Debug.LogWarning("Not grounded");
            moveDir = new Vector3(0, moveDir.y, vInput);
            moveDir = transform.TransformDirection(moveDir);
            moveDir.y -= gravity * Time.deltaTime;
        }
        charCon.Move(moveDir * Time.deltaTime);
    }

    private void DetectRotation()
    {
        if (hInput != 0)
        {
            Vector3 rotation = Vector3.up * hInput;
            transform.Rotate(rotation * Time.deltaTime);
        }
    }

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
        if(!_gameManager.playerInvincible)
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
        _gameManager.playerInvincible = true;
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
        _gameManager.playerInvincible = false;
    }
}
