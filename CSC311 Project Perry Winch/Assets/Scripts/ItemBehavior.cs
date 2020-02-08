using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            
            Destroy(this.transform.parent.gameObject);
            
            Debug.Log("Item collected!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
