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
        Hit,
        Die
    }

    [SerializeField] private int health;
    [SerializeField] private float runSpeed;
    [SerializeField] private float impairedSpeed;
    // Reference to the player GameObject or target object
    [SerializeField] private float frozenTime;
    
    
    private float _runningCounter;
    private Animator _greenPigAnimator;
    private Rigidbody2D _greenPigRigidbody2D;
    private BoxCollider2D _greenPigBoxCollider2D;
    private MonsterMovementState _monsterMovementState;
    private static readonly int MovementState = Animator.StringToHash("MovementState");
    private GameManager _gameManager;
    private static readonly int MovementStateHashCode = Animator.StringToHash("MovementState");
    private bool _isHit;
    private SpriteRenderer _greenPigSpriteRenderer;
    
    private int _directionX;
    private float _movementSpeed;
    private bool _isAlive;

    // Start is called before the first frame update
    void Start()
    {
        _greenPigAnimator = GetComponent<Animator>();
        _monsterMovementState = MonsterMovementState.Walking;
        _greenPigRigidbody2D = GetComponent<Rigidbody2D>();
        _greenPigBoxCollider2D = GetComponent<BoxCollider2D>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _greenPigSpriteRenderer = GetComponent<SpriteRenderer>();

        _isHit = false;

        _directionX = 1;
        _greenPigSpriteRenderer.flipX = _directionX > 0;
        _isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAlive)
        {
            MovementAI();
        }

        UpdateMovementStatus();
    }
    
    /// <summary>
    /// The target is set in Unity editor
    /// </summary>
    private void MovementAI()
    {
        _movementSpeed = _isHit ? impairedSpeed : runSpeed;
        
        _greenPigRigidbody2D.velocity = new Vector2(_movementSpeed * _directionX, _greenPigRigidbody2D.velocity.y);
    }

    private void UpdateMovementStatus()
    {
        if (_isAlive)
        {
            if (_isHit)
            {
                _monsterMovementState = MonsterMovementState.Hit;
            }
            else
            {
                _monsterMovementState = MonsterMovementState.Walking;
            }
        }
        else
        {
            _monsterMovementState = MonsterMovementState.Die;
        }
        
        ChangeAnimatorState();
    }

    private void ChangeAnimatorState()
    {
        switch (_monsterMovementState)
        {
            case MonsterMovementState.Walking:
                _greenPigAnimator.SetInteger(MovementStateHashCode, 0);
                break;
            case MonsterMovementState.Hit:
                _greenPigAnimator.SetInteger(MovementStateHashCode, 1);
                break;
            case MonsterMovementState.Die:
                _greenPigAnimator.SetInteger(MovementStateHashCode, 2);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dmg2Monster"))
        {
            TakeDmg();
        }

        if (other.CompareTag("ChangXDirectionTrigger"))
        {
            _directionX *= -1;
            _greenPigSpriteRenderer.flipX = _directionX > 0;
        }
    }

    private void Hit2WalkMovementStatus()
    {
        StartCoroutine(Hit2Walk());
    }

    private IEnumerator Hit2Walk()
    {
        yield return new WaitForSeconds(frozenTime);
        _isHit = false;
    }

    private void TakeDmg()
    {
        _isHit = true;
        
        Hit2WalkMovementStatus();

        if (health - 1 < 1)
        {
            _gameManager.OnKillPause();
            _isAlive = false;
            _movementSpeed = 0;

            _greenPigBoxCollider2D.isTrigger = true;
            _greenPigRigidbody2D.simulated = false;

            _greenPigSpriteRenderer.sortingLayerName = "Remains";
            _greenPigSpriteRenderer.sortingOrder = 0;
        }

        health--;
    }
}