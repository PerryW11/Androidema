using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public GameObject door;
    public Animator doorAnim;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "Player" || other.gameObject.CompareTag("Enemy"))
        {
            doorAnim.SetBool("IsNearDoor", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player" || other.gameObject.CompareTag("Enemy"))
        {
            doorAnim.SetBool("IsNearDoor", false);
        }
    }

}
