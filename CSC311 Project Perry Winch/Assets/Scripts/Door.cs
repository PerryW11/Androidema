using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public GameObject door;
    public Animator doorAnim;
    private bool isMoving;
    private bool doorOpen;

    private void OnTriggerStay(Collider other)
    {
        if(!isMoving && !doorOpen)
        {
            if (other.gameObject.name == "Player" || other.gameObject.CompareTag("Enemy"))
            {
                doorAnim.SetBool("IsNearDoor", true);                
                CallMovingDoor();
                doorOpen = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(!isMoving && doorOpen)
        {
            if (other.gameObject.name == "Player" || other.gameObject.CompareTag("Enemy"))
            {
                doorAnim.SetBool("IsNearDoor", false);
                CallMovingDoor();
            }
        }
        

    }

    private void CallMovingDoor()
    {
        StartCoroutine(MovingDoor());
    }

    IEnumerator MovingDoor()
    {
        isMoving = true;
        yield return new WaitForSeconds(1.5f);
        isMoving = false;
    
    }

}
