using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTriggerPad : MonoBehaviour
{

    public GameBehavior _gameManager;
    public GameBehavior.Levels myLevel;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            _gameManager.LoadLevel(myLevel);
        }
    }
}
