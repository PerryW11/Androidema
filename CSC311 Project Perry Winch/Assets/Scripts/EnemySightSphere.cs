using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightSphere : MonoBehaviour
{
    public EnemyBehavior scrEnemy;
    private Transform trnPlayer;
    private Vector3 origin;
    private Vector3 direction;

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
                if (hit)
                {
                    if (rayHit.transform.gameObject.name == "Player")
                    {
                        scrEnemy.HandlePlayerSight();

                    }
                }
            }
        }
    }
}
