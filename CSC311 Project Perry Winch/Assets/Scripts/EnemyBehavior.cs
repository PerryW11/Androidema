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

    public Animator charAnim;

    public Transform player;

    private NavMeshAgent agent;

    public GameObject gobExclamation;

    private int _lives = 3;

    private bool damagedPlayer = false;
    public bool DamagedPlayer
    {
        get
        {
            return damagedPlayer;
        }
    }
    public int EnemyLives
    {
        
        get { return _lives; }

        private set
        {
            _lives = value;
            if (_lives <= 0)
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
        InitializePatrolRoute();
        MoveToNextPatrolLocation();
    }
    void Update()
    {
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
        charAnim.SetBool("IsWalking", true);
        gobExclamation.SetActive(false);
    }

    public void HandlePlayerSight()
    {
        gobExclamation.SetActive(true);
        agent.destination = player.position;
        Debug.Log("Player detected - attack!");
    }

    public void HandlePlayerContact()
    {
        damagedPlayer = true;
        if(!gameManager.playerInvincible)
        {
            gameManager.Lives -= 1;
            MoveToNextPatrolLocation();
        }
    }

    void OnTriggerExit(Collider other)
    { 
        if (other.name == "Player")
        {
            Debug.Log("Player out of range, resume patrol");
            damagedPlayer = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Bullet(Clone)")
        {
            EnemyLives -= 1;
            Debug.Log("Enemy has been shot!");
        }
    }



}
