using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GreenPigControls : MonoBehaviour
{
    enum MonsterMovementState
    {
        Walking,
        Running
    }

    [SerializeField] private int health;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float patrolPeriod;

    [SerializeField] private float runSpeed;

    // Reference to the player GameObject or target object
    [SerializeField] private Transform target;
    [SerializeField] private float irritateDistance;

    private bool _hit;
    private float _patrolTimer; // It walks towards a random direction after a certain period.
    private int _patrolDirection;
    private float _runningCounter;
    private Animator _greenpigAnimator;
    private Rigidbody2D _greenpigRigidbody2D;
    private SpriteRenderer _greenpigSpriteRenderer;
    private MonsterMovementState _monsterMovementState;
    private static readonly int MovementState = Animator.StringToHash("MovementState");
    private GameManager _gameManager;


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
        
        _hit = false;
        _patrolTimer = 0;
        _patrolDirection = Random.Range(0, 2) == 0 ? -1 : 1;
        _greenpigAnimator = GetComponent<Animator>();
        _monsterMovementState = MonsterMovementState.Walking;
        _greenpigRigidbody2D = GetComponent<Rigidbody2D>();
        _greenpigSpriteRenderer = GetComponent<SpriteRenderer>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Chase player
        MovementAI();

        UpdateMovementState();
    }


    /// <summary>
    /// The target is set in Unity editor
    /// </summary>
    private void MovementAI()
    {
        
    }

    private void UpdateMovementState()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dmg2Monster"))
        {
            TakeDmg();
        }

        UpdateMovementState();
    }

    private void TakeDmg()
    {
        _hit = true;
        Debug.Log("Hit!");

        if (health - 1 < 1)
        {
            _gameManager.OnKillPause();
            Destroy(gameObject);
        }

        health--;
        _greenpigRigidbody2D.velocity = new Vector2(runSpeed / 4, 0);
    }
}