using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameBehavior : MonoBehaviour
{
    public int maxItems = 4;
    private int itemsCollected = 0;

    [System.NonSerialized] public bool showWinScreen = false;
    [System.NonSerialized] public bool showLossScreen = false;

    public float jumpVelocity = 5f;
    private float jumpLevel1 = 3f;
    private float jumpLevel2 = 5f;
    private float jumpLevel3 = 10f;
    private float myTimer;

    public TextMeshProUGUI txtJumpLevel;
    public TextMeshProUGUI txtItems;
    public TextMeshProUGUI txtLives;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtTime;
    public TextMeshProUGUI txtHighScore;

    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject levelPad;
    public GameObject levelPadDoor;
    public GameObject escMenu;
    public GameObject teleporterTip;

    public float pickupPitchMod = 0f; 
    public AudioSource audItemPickup;
    public AudioSource audNextLevelAvailable;
    public AudioSource audEnemyKilled;
    public AudioSource audPlayerKilled;


    public PlayerBehavior scrPlayer;
    public bool playerInvincible = false;
    public bool initialized = false;

    private bool gamePaused = false;

    public bool GamePaused
    {
        get { return gamePaused; }
    }

    public int Items // To handle the items collected
    {

        get { return itemsCollected; }

        set
        {
            itemsCollected = value;
            Debug.LogFormat("Items: {0}", itemsCollected);
            RefreshItemsText(itemsCollected); 
            PlayerPrefs.SetInt("ItemsCollected", value);

            if (itemsCollected >= MaxItemsAdjusted) // If the items collected is greater than or equal to the max items for that specific level
            {
                if (SceneManager.GetActiveScene().buildIndex == 0) // If it's level 1
                {
                    audNextLevelAvailable.Play();
                    ShowLevelPad();
                    teleporterTip.SetActive(true);
                }
                if (SceneManager.GetActiveScene().buildIndex == 1) // If it's level 2
                {
                    ShowWinScreen();
                }
            }
            else
            {
                if(itemsCollected != 0)
                {
                    if (initialized)
                    {
                        audItemPickup.pitch = 0.9f + pickupPitchMod; // Will change pitch of audItemPickup as more items are picked up
                        audItemPickup.Play();
                        pickupPitchMod += 0.15f;
                    }
                }
            }

        }
    }

    private int playerLives = 3;
    public int Lives
    {
        get { return playerLives; }
        set
        {

            if (value < playerLives)
            {
                if (initialized)
                {
                    scrPlayer.CallTempInvincibility(); // Temporary invincibility
                }
            }
            playerLives = value;
            PlayerPrefs.SetInt("PlayerLives", playerLives); 
            Debug.LogFormat("Lives: {0}", playerLives);
            RefreshLivesText(playerLives);

            if (playerLives <= 0)
            {
                audPlayerKilled.Play();
                ShowLoseScreen();
            }
        }


    }

    public int MaxItemsAdjusted
    {
        get
        {
            int ret = maxItems;
            if (SceneManager.GetActiveScene().buildIndex == 1) // If it's level 2
            {
                ret *= 2; // Double the items required to win
            }
            return ret;
        }
    }


    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // If it's level 1
        {
            Items = 0;
            Lives = 3;
        }
        else
        {
            // Checking for data to carry over in level
            if (PlayerPrefs.HasKey("ItemsCollected"))
            {
                Items = PlayerPrefs.GetInt("ItemsCollected");
            }
            if (PlayerPrefs.HasKey("PlayerLives"))
            {
                Lives = PlayerPrefs.GetInt("PlayerLives");
            }
            if(PlayerPrefs.HasKey("TimeCompleted"))
            {
                myTimer = PlayerPrefs.GetFloat("TimeCompleted");
            }
        }
        RefreshJumpText(2);
        RefreshItemsText(Items);
        RefreshLivesText(Lives);
        RefreshLevelText(SceneManager.GetActiveScene().buildIndex + 1);
        initialized = true; // Scene is fully initialized and setup
    }

    private void Update()
    {
        if(!winScreen.activeInHierarchy || !loseScreen.activeInHierarchy || !escMenu.activeInHierarchy)
        {
            myTimer += Time.deltaTime;
            RefreshTimeText(myTimer);
            PlayerPrefs.SetFloat("TimeCompleted", myTimer);
            RefreshHighScoreText(myTimer);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escMenu.activeInHierarchy)
            {
                escMenu.SetActive(false);
            }
            else
            {
                ShowEscMenu();
            }
        }
    }

    public enum Levels
    {
        Level_1,
        Level_2
    }

    public void EnemyKilled()
    {
        audEnemyKilled.Play();
    }

    private void ShowLevelPad()
    {
        levelPad.SetActive(true);
        levelPadDoor.SetActive(false);
    }
    private void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
        Time.timeScale = 0;
    }
    private void ShowWinScreen()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
    }

    private void ShowEscMenu()
    {
        escMenu.SetActive(true);
    }

    public void QuitGame()
    {
       #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
       #else
        {
            Application.Quit();
        }
       #endif
    }
    
    public void LoadLevel(Levels myLevel)
    {
        SceneManager.LoadScene((int)myLevel);
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

    private void RefreshItemsText(int x)
    {
        txtItems.text = "Items: " + x.ToString() + "/" + MaxItemsAdjusted.ToString();
    }

    private void RefreshLivesText(int x)
    {
        txtLives.text = "Lives: " + x.ToString() + "/3";
    }

    private void RefreshLevelText(int x)
    {
        txtLevel.text = "Level " + x.ToString();
    }

    private void RefreshTimeText(float x)
    {
        txtTime.text = "Time: " + x.ToString("#.000");
    }

    private void RefreshHighScoreText(float x)
    {
        txtHighScore.text = "High Score: " + x.ToString("#.000");
    }

    public void ClearHighScore()
    {
        PlayerPrefs.SetFloat("TimeCompleted", 0f);
        RefreshHighScoreText(PlayerPrefs.GetFloat("TimeCompleted"));
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
