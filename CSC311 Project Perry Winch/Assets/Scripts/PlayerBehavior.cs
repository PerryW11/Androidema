using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBehavior : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 90f;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;
    private float gravity = 8f;

    public GameObject bullet;
    public float bulletSpeed = 60f;

    private float vInput;
    private float hInput;
    private Vector3 moveDir;

    //private Rigidbody _rb;
    private CapsuleCollider _col;
    private CharacterController charCon;
    private GameBehavior _gameManager;
 
    private void Start()
    {
       // _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        charCon = GetComponent<CharacterController>();
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameBehavior>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Enemy")
        {
            _gameManager.Lives -= 1;
        }
    }

    private void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * rotateSpeed;

        DetectMovement();
        DetectRotation();
        DetectFire();
        
    }

    private void DetectMovement()
    {
        if (charCon.isGrounded)
        {
            Debug.Log("Grounded");
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
            Debug.LogWarning("Not grounded");
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
        if (Input.GetMouseButtonDown(0) && !(EventSystem.current.IsPointerOverGameObject()))
        {
            GameObject newBullet = Instantiate(bullet, this.transform.position + (this.transform.forward * 1.2f), this.transform.rotation) as GameObject;
            Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
            bulletRB.velocity = this.transform.forward * bulletSpeed;
        }
    }
}
