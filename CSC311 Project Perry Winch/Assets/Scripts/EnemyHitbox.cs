using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public EnemyBehavior scrEnemy;

    
    private void OnTriggerEnter(Collider other)
    {
        //If the player was just hit by the enemy
        if (other.gameObject.name == "Player")
        {
            scrEnemy.HandlePlayerContact();
        }
    }
}
