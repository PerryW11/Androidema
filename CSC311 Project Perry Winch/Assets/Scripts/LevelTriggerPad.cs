using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTriggerPad : MonoBehaviour
{

    public GameBehavior gameManager;
    public GameBehavior.Levels myLevel;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            
            gameManager.RefreshTimeBeatenText(gameManager.myTimer);
            float bestTime = PlayerPrefs.GetFloat("BestTime");
            float timeCompleted = PlayerPrefs.GetFloat("TimeCompleted");
            if (bestTime >= timeCompleted || bestTime == 0)
            {
                PlayerPrefs.SetFloat("BestTime", timeCompleted);
                gameManager.RefreshBestTimeText(timeCompleted);
            }
            else
            {

                gameManager.RefreshBestTimeText(bestTime);
            }
            gameManager.ShowWinScreen();
            
        }
    }
}
