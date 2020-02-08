using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float moveSpeed = 10f;
    public float rotateSpeed = 75f;
    
    private float vInput;
    private float hInput;

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;

        hInput = Input.GetAxis("Horizontal") * rotateSpeed;
    }
    void FixedUpdate()
        {
            Vector3 rotation = Vector3.up * hInput;

            Quaternion angleRot = Quaternion.Euler(rotation *
            Time.fixedDeltaTime);

            _rb.MovePosition(this.transform.position +
           this.transform.forward * vInput * Time.fixedDeltaTime);
      
            _rb.MoveRotation(_rb.rotation * angleRot);
        }
    }
