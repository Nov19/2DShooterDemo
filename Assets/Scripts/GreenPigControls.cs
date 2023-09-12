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
        if (target is null)
            return;

        // Calculate the direction towards the target
        float directionX2Go = target.position.x - transform.position.x;
        // Change sprite flipX
        _greenpigSpriteRenderer.flipX = directionX2Go > 0;

        float speed;
        if (_hit && _runningCounter <= 3000)
        {
            speed = runSpeed;
            _runningCounter++;
            _monsterMovementState = MonsterMovementState.Running;
        }
        else
        {
            _hit = false;
            _runningCounter = 0;
            speed = walkSpeed;
            _monsterMovementState = MonsterMovementState.Walking;
        }

        // Move the AI towards the player when they're close enough and not too close
        if (directionX2Go is < -0.2f or > 0.2f)
        {
            if (directionX2Go < irritateDistance)
            {
                _greenpigRigidbody2D.velocity = directionX2Go > 0 ? new Vector2(speed, _greenpigRigidbody2D.velocity.y) : new Vector2(-speed, _greenpigRigidbody2D.velocity.y);
            }
        }
        
        if (directionX2Go > irritateDistance)
        //if (directionX2Go > irritateDistance && Time.time > _patrolTimer)
        {
            // This makes the monster moving toward a random direction.
            if (Time.time > _patrolTimer)
            {
                _patrolTimer += patrolPeriod;
                _patrolDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            }
            
            _greenpigRigidbody2D.velocity = new Vector2(_patrolDirection * patrolSpeed, _greenpigRigidbody2D.velocity.y);
        }
    }

    private void UpdateMovementState()
    {
        switch (_monsterMovementState)
        {
            case MonsterMovementState.Walking:
                _greenpigAnimator.SetInteger(MovementState, 0);
                break;
            case MonsterMovementState.Running:
                _greenpigAnimator.SetInteger(MovementState, 1);
                break;
        }
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

        if (health - 1 < 1)
            Destroy(this.gameObject);

        health--;
        _greenpigRigidbody2D.velocity = new Vector2(runSpeed / 4, 0);
    }
}