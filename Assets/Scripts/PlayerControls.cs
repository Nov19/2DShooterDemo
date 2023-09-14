using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    // Main character's constant status
    private static readonly int MovementStateHashCode = Animator.StringToHash("MovementState");

    // Serialized fields
    [SerializeField] private GameObject gameManager;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dmg2UpperForce;
    [SerializeField] private float onFireRecoil;
    [SerializeField] private GameObject weapons;
    [SerializeField] private GameObject stepTrigger;


    // Main character's dynamic status
    private bool _alive;
    private bool _canJump;
    private int _health;
    private float _directionX;
    private bool _playerIsFiring;
    private bool _isFacingRight;

    // Helper values
    private readonly float _yVelocityThreshold = 0.1f;

    // Player's components
    private float _muzzleFlamePositiveXPosition;
    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;
    private SpriteRenderer _playerSpriteRenderer;
    private MovementState _playerMovementState;
    private GameObject _gameManager;
    private GunControls _gunScript;
    private StepTrigger _stepTriggerScript;
    private bool _shouldNotHurt;

    // Enums
    private enum MovementState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Firing,
        Die
    }


    // Start is called before the first frame update
    void Start()
    {
        _alive = true;
        _health = 1;
        _directionX = 0f;
        _playerIsFiring = false;
        _canJump = true;
        _playerMovementState = MovementState.Idle;

        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _playerSpriteRenderer = GetComponent<SpriteRenderer>();
        _gameManager = GameObject.Find("GameManager");

        _gunScript = weapons.GetComponent<GunControls>();
        _gunScript.OnStopFire += FireIsOff;

        _stepTriggerScript = stepTrigger.GetComponent<StepTrigger>();
        _stepTriggerScript.OnStepEnter += ShouldNotTakeDmg;
        _stepTriggerScript.OnStepExit += ShouldTakeDmg;
        // Debug MSG
        // Debug.Log("Player initialization succeed!");
    }

    // Update is called once per frame
    void Update()
    {
        // Enable player's controls when the character is alive
        if (_alive)
        {
            _directionX = Input.GetAxis("Horizontal");
            // Move towards left/right
            MoveLeftOrRight();

            // Jump
            if (Input.GetButtonDown("Jump") && _canJump)
            {
                Jump();
            }

            // Fire
            // If the fire button is pressed and the player is not currently firing
            if (Input.GetButton("Fire1"))
            {
                // Debug.Log("Fire!");
                if (!_playerIsFiring)
                {
                    _playerIsFiring = true;
                    _gunScript.FireOn();
                }
            }

            // If the fire button is released
            if (Input.GetButtonUp("Fire1"))
            {
                _playerIsFiring = false;
                _gunScript.FireOff();
            }

            if (Input.GetButtonDown("Special1"))
            {
                _gunScript.EnableDoubleShot();
            }
        }

        UpdatePlayerMovementStatus();
    }

    private void UpdatePlayerMovementStatus()
    {
        // Firing suppresses all other states
        if (!_alive)
        {
            _playerMovementState = MovementState.Die;
        }
        else if (_playerIsFiring)
        {
            _playerMovementState = MovementState.Firing;
        }
        else
        {
            // Check if character is on air
            float velocityY = _playerRigidbody2D.velocity.y;
            if (Math.Abs(velocityY) > _yVelocityThreshold)
            {
                if (velocityY > 0)
                {
                    _playerMovementState = MovementState.Jumping;
                }
                else
                {
                    _playerMovementState = MovementState.Falling;
                }
            }
            else
            {
                // If the character is not on air, check whether it is moving
                if (Math.Abs(_playerRigidbody2D.velocity.x) > _yVelocityThreshold)
                {
                    _playerMovementState = MovementState.Walking;
                }
                else
                {
                    _playerMovementState = MovementState.Idle;
                }
            }
        }
        
        ChangeAnimatorState();
    }

    private void ChangeAnimatorState()
    {
        switch (_playerMovementState)
        {
            case MovementState.Idle:
                _playerAnimator.SetInteger(MovementStateHashCode, 0);
                break;
            case MovementState.Walking:
                _playerAnimator.SetInteger(MovementStateHashCode, 1);
                break;
            case MovementState.Jumping:
                _playerAnimator.SetInteger(MovementStateHashCode, 2);
                break;
            case MovementState.Falling:
                _playerAnimator.SetInteger(MovementStateHashCode, 3);
                break;
            case MovementState.Firing:
                _playerAnimator.SetInteger(MovementStateHashCode, 4);
                break;
            case MovementState.Die:
                _playerAnimator.SetInteger(MovementStateHashCode, 5);
                break;
            default:
                throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Invoke when players press directional keys (a,d,left,right)
    /// </summary>
    private void MoveLeftOrRight()
    {
        // Flip the character and the weapon based on moving direction only when player is not firing
        if (_directionX != 0 && !_playerIsFiring)
        {
            // Set player's sprites flip
            _isFacingRight = _directionX > 0;

            _playerSpriteRenderer.flipX = _isFacingRight;
        }

        Vector2 newVelocity;
        if (_playerIsFiring)
        {
            newVelocity = new Vector2(_directionX * movementSpeed * 0.8f, _playerRigidbody2D.velocity.y);
        }
        else
        {
            newVelocity = new Vector2(_directionX * movementSpeed, _playerRigidbody2D.velocity.y);
        }
        _playerRigidbody2D.velocity = newVelocity;
    }

    private void Jump()
    {
        _canJump = false;
        _playerRigidbody2D.velocity = new Vector2(_playerRigidbody2D.velocity.x, jumpForce);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dmg2Player"))
        {
            TakeDmg();
        }
    }

    private void TakeDmg()
    {
        // When stepped on enemies...
        if (_shouldNotHurt)
            return;
        
        _playerRigidbody2D.velocity = new Vector2(0, dmg2UpperForce);
        _health--;

        if (_health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Call this when player should be killed
    /// </summary>
    public void Die()
    {
        _alive = false;

        _playerRigidbody2D.bodyType = RigidbodyType2D.Static;

        _gunScript.FireOff();

        GameManager manager =_gameManager.GetComponent<GameManager>();
            
        manager.GameOver();
        
        manager.GameOverSloMo();
        
        Destroy(gameObject, 0.5f);
    }
    
    

    /// <summary>
    /// This will be called by StepTrigger.cs when players step on monsters.
    /// </summary>
    public void StepOnMonster()
    {
        Vector2 curVelocity = _playerRigidbody2D.velocity;
        _playerRigidbody2D.velocity = new Vector2(curVelocity.x, jumpForce / 1.5f);
    }

    /// <summary>
    /// enable jumping when players touch any grounds.
    /// </summary>
    public void TouchGround()
    {
        _canJump = true;
    }

    public bool IsFacingRight()
    {
        return _isFacingRight;
    }

    public void FireRecoil()
    {
        Vector3 newPos = this.gameObject.transform.position;

        if (_isFacingRight)
        {
            newPos -= new Vector3(onFireRecoil, 0, 0);
        }
        else
        {
            newPos += new Vector3(onFireRecoil, 0, 0);
        }

        transform.position = newPos;
    }

    private void FireIsOff()
    {
        _playerIsFiring = false;
    }

    private void ShouldNotTakeDmg()
    {
        _shouldNotHurt = true;
    }
    
    private void ShouldTakeDmg()
    {
        _shouldNotHurt = false;
    }
}