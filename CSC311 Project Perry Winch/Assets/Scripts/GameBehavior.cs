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
    public TextMeshProUGUI txtItems;
    public TextMeshProUGUI txtLives;
    public GameObject winScreen;
    public GameObject loseScreen;


    private void Start()
    {
        RefreshJumpText(2);
        RefreshItemsText(0);
        RefreshLivesText(3);
    }

    public int Items
    {

        get { return _itemsCollected; }

        set
        {
            _itemsCollected = value;
            Debug.LogFormat("Items: {0}", _itemsCollected);
            RefreshItemsText(_itemsCollected);

            if (_itemsCollected >= maxItems)
            {
                ShowWinScreen();
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
            RefreshLivesText(_playerLives);

            if (_playerLives <= 0)
            {
                ShowLoseScreen();
            }
            else
            {
                labelText = "Ouch... that's got hurt.";
            }
        }


    }

    public void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
        Time.timeScale = 0;
    }
    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
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

    public void RefreshItemsText(int x)
    {
        txtItems.text = "Items: " + x.ToString();
    }

    public void RefreshLivesText(int x)
    {
        txtLives.text = "Lives: " + x.ToString();
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
