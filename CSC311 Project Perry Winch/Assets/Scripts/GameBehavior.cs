using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;
public class GameBehavior : MonoBehaviour
{
    public string labelText = "Collect all 4 items and win your freedom!";
    public int maxItems = 4;

    public bool showWinScreen = false;
    private int _itemsCollected = 0;

    public bool showLossScreen = false;

    public float jumpVelocity = 5f;
    public float jumpLevel1 = 3f;
    public float jumpLevel2 = 5f;
    public float jumpLevel3 = 10f;
    public TextMeshProUGUI txtJumpLevel;


    private void Start()
    {
        RefreshJumpText(2);
    }

    public int Items
    {

        get { return _itemsCollected; }

        set
        {
            _itemsCollected = value;
            Debug.LogFormat("Items: {0}", _itemsCollected);

            if (_itemsCollected >= maxItems)
            {
                labelText = "You've found all the items!";
                showWinScreen = true;

                Time.timeScale = 0f;
            }
            else
            {
                labelText = "Item found, only " + (maxItems -
                _itemsCollected) + " more to go!";
            }

        }
    }
    private int _playerLives = 3;

    public int Lives
    {
        get { return _playerLives; }
        set
        {
            _playerLives = value;
            Debug.LogFormat("Lives: {0}",
            _playerLives);

            if (_playerLives <= 0)
            {
                labelText = "You want another life with that?";
                showLossScreen = true;
                Time.timeScale = 0;
            }
            else
            {
                labelText = "Ouch... that's got hurt.";
            }
        }


    }

    void RestartLevel()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
    }

    void OnGUI()
    {
        // 4
        GUI.Box(new Rect(20, 20, 150, 25), "Player lives: " + _playerLives);

        GUI.Box(new Rect(20, 50, 150, 25), "Items Collected: " + _itemsCollected);

        GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 50, 300, 50), labelText);

        if (showWinScreen)
        {
            // 4
            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), "YOU WON!"))
            {
                RestartLevel();
            }

        }
        if (showLossScreen)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 100,
            Screen.height / 2 - 50, 200, 100), "You lose..."))
            {
                RestartLevel();
            }
        }
    }
       
    public void RefreshJumpText(int x)
    {
        switch(x)
        {
            case 1:
                txtJumpLevel.text = "Jump Force: " + jumpVelocity.ToString("#.00");
                break;
            case 2:
                txtJumpLevel.text = "Jump Force: " + jumpVelocity.ToString("#.00");
                break;
            case 3:
                txtJumpLevel.text = "Jump Force: " + jumpVelocity.ToString("#.00");
                break;
            default:
                Debug.LogError("Unsupported jump force level");
                break;

        }
    }

    public void ClickedJumpIncrease()
    {
        if(jumpVelocity == jumpLevel1)
        {
            jumpVelocity = jumpLevel2;
            RefreshJumpText(2); 
        }
        else if (jumpVelocity == jumpLevel2)
        {
            jumpVelocity = jumpLevel3;
            RefreshJumpText(3);
        }
        else
        {
            Debug.Log("Already at MAX jump");
        }
    }
    public void ClickedJumpDecrease()
    {
        if (jumpVelocity == jumpLevel3)
        {
            jumpVelocity = jumpLevel2;
            RefreshJumpText(2);
        }
        else if (jumpVelocity == jumpLevel2)
        {
            jumpVelocity = jumpLevel1;
            RefreshJumpText(3);
        }
        else
        {
            Debug.Log("Already at MIN jump");
        }
    }

}
