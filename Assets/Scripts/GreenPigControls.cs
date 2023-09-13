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
        Hit
    }

    [SerializeField] private int health;
    [SerializeField] private float runSpeed;
    // Reference to the player GameObject or target object
    [SerializeField] private float frozenTime;
    
    
    private float _runningCounter;
    private Animator _greenPigAnimator;
    private Rigidbody2D _greenPigRigidbody2D;
    private MonsterMovementState _monsterMovementState;
    private static readonly int MovementState = Animator.StringToHash("MovementState");
    private GameManager _gameManager;
    private static readonly int MovementStateHashCode = Animator.StringToHash("MovementState");
    private bool _isHit;
    private SpriteRenderer _greenPigSpriteRenderer;
    
    private int _directionX;

    // Start is called before the first frame update
    void Start()
    {
        _greenPigAnimator = GetComponent<Animator>();
        _monsterMovementState = MonsterMovementState.Walking;
        _greenPigRigidbody2D = GetComponent<Rigidbody2D>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _greenPigSpriteRenderer = GetComponent<SpriteRenderer>();

        _isHit = false;

        _directionX = 1;
        _greenPigSpriteRenderer.flipX = _directionX > 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Chase player
        MovementAI();

        UpdateMovementStatus();
    }
    
    /// <summary>
    /// The target is set in Unity editor
    /// </summary>
    private void MovementAI()
    {
        _greenPigRigidbody2D.velocity = new Vector2(runSpeed * _directionX, _greenPigRigidbody2D.velocity.y);
    }

    private void UpdateMovementStatus()
    {
        if (_isHit)
        {
            _monsterMovementState = MonsterMovementState.Hit;
        }
        else
        {
            _monsterMovementState = MonsterMovementState.Walking;
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
            Destroy(gameObject);
        }

        health--;
        _greenPigRigidbody2D.velocity = new Vector2(runSpeed / 4, 0);
    }
}