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

    // status values


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

        // Debug MSG
        Debug.Log("Player initialization succeed!");
    }

    // Update is called once per frame
    void Update()
    {
        // Enable player's controls when the character is alive
        if (_alive)
        {
            // Move towards left/right
            _directionX = Input.GetAxis("Horizontal");
            MoveLeftOrRight();

            // Jump
            if (Input.GetButtonDown("Jump") && _canJump)
            {
                Jump();
            }

            // Fire
            // If the fire button is pressed and the player is not currently firing
            if (Input.GetButton("Fire1") && !_playerIsFiring)
            {
                _playerIsFiring = true;
                _gunScript.FireOn();
            }

            // If the fire button is released
            if (Input.GetButtonUp("Fire1"))
            {
                _playerIsFiring = false;
                _gunScript.FireOff();
            }

            UpdatePlayerMovementStatus();
        }
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
                Debug.Log("Jumping or Falling!");
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
                    Debug.Log("Walking");
                    _playerMovementState = MovementState.Walking;
                }
                else
                {
                    Debug.Log("Idle");
                    _playerMovementState = MovementState.Idle;
                }
            }
        }

        // TODO:
        ChangeAnimatorState();
    }

    private void ChangeAnimatorState()
    {
        switch (_playerMovementState)
        {
            
        }
    }


    /// <summary>
    /// Invoke when players press directional keys (a,d,left,right)
    /// </summary>
    private void MoveLeftOrRight()
    {
        // Flip the character and the weapon based on moving direction
        if (_directionX != 0)
        {
            // Set player's sprites flip
            _isFacingRight = _directionX > 0;

            _playerSpriteRenderer.flipX = _isFacingRight;
        }

        _playerRigidbody2D.velocity = new Vector2(_directionX * movementSpeed, _playerRigidbody2D.velocity.y);
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

        // UpdatePlayerMovementState();

        _gameManager.GetComponent<GameManager>().GameOver();
    }

    /// <summary>
    /// This will be called by StepTrigger.cs when players step on monsters.
    /// </summary>
    public void StepOnMonster()
    {
        _playerRigidbody2D.velocity = new Vector2(_playerRigidbody2D.velocity.x, jumpForce / 2);
    }

    /// <summary>
    /// enable jumping when players touch any grounds.
    /// </summary>
    public void TouchGround()
    {
        _canJump = true;
    }

    public bool isFacingRight()
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
}