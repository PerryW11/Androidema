using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{

    public GameBehavior gameManager;
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameBehavior>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Destroy(transform.parent.gameObject);
            Debug.Log("Item collected!");
            gameManager.Rodon += 1;
        }
    }
}
