using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameBehavior : MonoBehaviour
{
    public int maxRodon = 4;
    private int rodonCollected = 0;

    [System.NonSerialized] public bool showWinScreen = false;
    [System.NonSerialized] public bool showLossScreen = false;

    public float jumpVelocity = 5f;
    private float jumpLevel1 = 3f;
    private float jumpLevel2 = 5f;
    private float jumpLevel3 = 10f;
    private float myTimer;

    public TextMeshProUGUI txtJumpLevel;
    public TextMeshProUGUI txtRodon;
    public TextMeshProUGUI txtLives;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtTime;
    public TextMeshProUGUI txtTimeBeat;
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

    public int Rodon // To handle the rodon collected
    {

        get { return rodonCollected; }

        set
        {
            rodonCollected = value;
            Debug.LogFormat("Rodon: {0}", rodonCollected);
            RefreshRodonText(rodonCollected); 
            PlayerPrefs.SetInt("RodonCollected", value);

            if (rodonCollected >= MaxRodonAdjusted) // If the rodon collected is greater than or equal to the max rodon for that specific level
            {
                if (SceneManager.GetActiveScene().buildIndex == 0) // If it's level 1
                {
                    audNextLevelAvailable.Play();
                    ShowLevelPad();
                    teleporterTip.SetActive(true);
                }
                if (SceneManager.GetActiveScene().buildIndex == 1) // If it's level 2
                {
                    RefreshTimeBeatenText(myTimer);
                    float bestTime = PlayerPrefs.GetFloat("BestTime");
                    float timeCompleted = PlayerPrefs.GetFloat("TimeCompleted");
                    if (bestTime >= timeCompleted || bestTime == 0)
                    {
                        PlayerPrefs.SetFloat("BestTime", timeCompleted);
                        RefreshBestTimeText(timeCompleted);
                    }
                    else
                    {

                        RefreshBestTimeText(bestTime);
                    }              
                    ShowWinScreen();
                }
            }
            else
            {
                if(rodonCollected != 0)
                {
                    if (initialized)
                    {
                        audItemPickup.pitch = 0.9f + pickupPitchMod; // Will change pitch of audItemPickup as more rodon is picked up
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

    public int MaxRodonAdjusted
    {
        get
        {
            int ret = maxRodon;
            if (SceneManager.GetActiveScene().buildIndex == 1) // If it's level 2
            {
                ret *= 2; // Double the rodon required to win
            }
            return ret;
        }
    }


    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // If it's level 1
        {
            Rodon = 0;
            Lives = 3;
        }
        else
        {
            // Checking for data to carry over in level
            if (PlayerPrefs.HasKey("RodonCollected"))
            {
                Rodon = PlayerPrefs.GetInt("RodonCollected");
            }
            if (PlayerPrefs.HasKey("PlayerLives"))
            {
                Lives = PlayerPrefs.GetInt("PlayerLives");
            }
            if(PlayerPrefs.HasKey("TimeCompleted"))
            {
                myTimer = PlayerPrefs.GetFloat("TimeCompleted");
            }

            if(PlayerPrefs.HasKey("BestTime"))
            {
                Debug.Log("Best time: " + PlayerPrefs.GetFloat("BestTime"));
            }
            else
            {
                PlayerPrefs.SetFloat("BestTime", 0f);
            }
        }
        RefreshJumpText(2);
        RefreshRodonText(Rodon);
        RefreshLivesText(Lives);
        RefreshLevelText(SceneManager.GetActiveScene().buildIndex + 1);
        initialized = true; // Scene is fully initialized and setup
    }

    private void Update()
    {
        if(!winScreen.activeInHierarchy || !loseScreen.activeInHierarchy || !escMenu.activeInHierarchy)
        {
            gamePaused = false;
            if(rodonCollected < MaxRodonAdjusted || SceneManager.GetActiveScene().buildIndex == 1) // To prevent race condition
            {
                myTimer += Time.deltaTime; // Increase timer for mission
                RefreshTimeText(myTimer);
                PlayerPrefs.SetFloat("TimeCompleted", myTimer);
            }
        }
        else
        {
            gamePaused = true;
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
        if(gamePaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
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
    }
    private void ShowWinScreen()
    {
        winScreen.SetActive(true);
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

    private void RefreshRodonText(int x)
    {
        txtRodon.text = "Rodon collected: " + x.ToString() + "/" + MaxRodonAdjusted.ToString();
    }

    private void RefreshLivesText(int x)
    {
        txtLives.text = "Total lives left: " + x.ToString() + "/3";
    }

    private void RefreshLevelText(int x)
    {
        txtLevel.text = "Level " + x.ToString();
    }

    private void RefreshTimeText(float x)
    {
        txtTime.text = "Time: " + x.ToString("#.000");
    }

    private void RefreshTimeBeatenText(float x)
    {
        txtTimeBeat.text = "Mission Time: " + x.ToString("#.000");
    }

    private void RefreshBestTimeText(float x)
    {
        txtHighScore.text = "Best Time: " + x.ToString("#.000");
    }

    public void ClearHighScore()
    {
        PlayerPrefs.SetFloat("TimeCompleted", 0f);
        RefreshBestTimeText(PlayerPrefs.GetFloat("TimeCompleted"));
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
