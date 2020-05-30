using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightSphere : MonoBehaviour
{
    public EnemyBehavior scrEnemy;
    private Transform trnPlayer;
    private Vector3 origin;
    private Vector3 direction;
    public bool playerFound = false;

    private void Start()
    {
        trnPlayer = FindObjectOfType<PlayerBehavior>().transform;
    }

    private void Update()
    {
        origin = transform.position;
        direction = trnPlayer.transform.position - origin;
        Debug.DrawRay(origin, direction, Color.red);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!scrEnemy.DamagedPlayer)
        {
            if (other.gameObject.name == "Player")
            {
                Ray ray = new Ray(origin, direction);
                bool hit = Physics.Raycast(ray, out RaycastHit rayHit);
                //If the player is in the line of sight of the enemy
                if (hit)
                {
                    if (rayHit.transform.gameObject.name == "Player")
                    {
                        playerFound = true;
                        scrEnemy.HandlePlayerSight();
                    }
                    else
                    {
                        playerFound = false;
                    }
                }
            }
        }
    }
}
