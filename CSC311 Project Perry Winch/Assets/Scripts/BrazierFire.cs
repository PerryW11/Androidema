using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrazierFire : MonoBehaviour
{
    public GameObject fireOne;
    public GameObject fireTwo;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            fireOne.SetActive(true);
            fireTwo.SetActive(true);
        }
    }
}
