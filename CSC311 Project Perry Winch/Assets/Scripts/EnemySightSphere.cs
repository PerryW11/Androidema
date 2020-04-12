using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightSphere : MonoBehaviour
{
    public EnemyBehavior scrEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            scrEnemy.HandlePlayerSight();
        }
    }
}
