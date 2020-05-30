using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public Transform patrolRoute;
    public List<Transform> locations;
    private int locationIndex = 0;
    private GameBehavior gameManager;
    private EnemySightSphere sight;

    public Animator charAnim;
    public Transform player;
    private PlayerBehavior scrPlayer;
    private NavMeshAgent agent;
    public GameObject gobExclamation;
    public AudioSource audHit;

    private int hitCounter = 0;

    private int lives = 5;

    private bool damagedPlayer = false; //If the player was just hit by the enemy
    public bool DamagedPlayer
    {
        get
        {
            return damagedPlayer;
        }
    }
    public int EnemyLives
    {
        
        get { return lives; }

        private set
        {
            lives = value;
            if (lives <= 0)
            {
                gameManager.EnemyKilled();
                Destroy(this.gameObject);
                Debug.Log("Enemy down.");
            }
        }
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameBehavior>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        scrPlayer = GameObject.Find("Player").GetComponent<PlayerBehavior>();
        InitializePatrolRoute();
        MoveToNextPatrolLocation();
        sight = GetComponentInChildren<EnemySightSphere>();
    }
    void Update()
    {
        //Sprint when player is seen, walk when player isn't seen
        if (sight.playerFound)
        {
            agent.speed = 5.5f;
        }
        else
        {
            agent.speed = 2f;
        }

        if (agent.speed == 2)
        {
            charAnim.SetBool("IsWalking", true);
            charAnim.SetBool("IsRunning", false);
        }
        if(agent.speed == 5.5f)
        {
            charAnim.SetBool("IsRunning", true);
            charAnim.SetBool("IsWalking", false);
        }
        if (agent.remainingDistance < 0.2f && !agent.pathPending)
        {
            MoveToNextPatrolLocation();
        }
    }

    void InitializePatrolRoute()
    {
        foreach (Transform child in patrolRoute)
        {
            locations.Add(child);
        }
    }

    void MoveToNextPatrolLocation()
    {
        if (locations.Count == 0)
            return;

        
        agent.destination = locations[locationIndex].position;
        locationIndex = (locationIndex + 1) % locations.Count;
        gobExclamation.SetActive(false);
    }

    //What the enemy does if the player is seen
    public void HandlePlayerSight() 
    {
        gobExclamation.SetActive(true);
        agent.destination = player.position;
        Debug.Log("Player detected - attack!");
        sight.playerFound = true;
    }

    public void HandlePlayerContact()
    {
        damagedPlayer = true;
        if(!gameManager.playerInvincible)
        {
            gameManager.Lives -= 1;
            //To play player hurt sounds as long as it's not the last life being lost
            if(gameManager.Lives != 0)
            {
                if (hitCounter == 0)
                {
                    scrPlayer.audPlayerHurtOne.Play();
                }
                if (hitCounter == 1)
                {
                    scrPlayer.audPlayerHurtTwo.Play();
                    hitCounter = -1;
                }
                hitCounter++;
            }
            MoveToNextPatrolLocation();
        }
    }

    void OnTriggerExit(Collider other)
    { 
        //Player is out of sight sphere
        if (other.name == "Player")
        {
            Debug.Log("Player out of range, resume patrol");
            damagedPlayer = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        //If shot by the player
        if (collision.gameObject.name == "Bullet(Clone)")
        {
            EnemyLives -= 1;
            if(EnemyLives != 0)
            {
                audHit.Play();
            }
            Debug.Log("Enemy has been shot!");
        }
    }



}
