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
    [SerializeField] private float bulletSpawnRate;
    [SerializeField] private GameObject[] hearts;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float[] bulletPosition;

    // Main character's dynamic status
    private bool _alive;
    private bool _canJump;
    private int _health;
    private float _directionX;
    private bool _isFiring;
    private float _nextFire;
    private float _muzzleFlamePositiveXPosition;
    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;
    private SpriteRenderer _playerSpriteRenderer;
    private SpriteRenderer[] _weaponSpriteRenderers = new SpriteRenderer[2];
    private MovementState _playerMovementState;
    private GameObject _gameManager;
    private bool _isFacingRight;
    
    // Enums
    private enum MovementState
    {
        Idle, Walking, Jumping, Falling, Firing, Die
    }


    // Start is called before the first frame update
    void Start()
    {
        _alive = true;
        _health = 1;
        _directionX = 0f;
        _isFiring = false;
        _nextFire = 0.0f;
        _canJump = true;
        _playerMovementState = MovementState.Idle;

        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _playerSpriteRenderer = GetComponent<SpriteRenderer>();
        _gameManager = GameObject.Find("GameManager");

        // Initialize weapon's sprite renderers
        for (int i = 0; i < _weaponSpriteRenderers.Length; i++)
        {
            _weaponSpriteRenderers[i] = weapons[i].GetComponent<SpriteRenderer>();
        }

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
            if (Input.GetButton("Fire1"))
            {
                Shooting(true);
            }
            else
            {
                Shooting(false);
            }
        }
        
        UpdatePlayerMovementState();
    }

    /// <summary>
    /// Control shooting the visibility of the gun
    /// </summary>
    /// <param name="fire"></param>
    private void Shooting(bool fire)
    {
        // Let UpdatePlayerMovementState() know that we are shooting.
        _isFiring = fire;

        // Make the gun visible
        for(int i = 0; i < _weaponSpriteRenderers.Length; i++)
        {
            weapons[i].gameObject.SetActive(fire);
        }

        if (fire && Time.time > _nextFire)
        {
            _nextFire += bulletSpawnRate;
            
            // Bullet spawn position
            if (_playerSpriteRenderer.flipX)
            {
                Instantiate(bulletPrefab, transform.position + new Vector3(bulletPosition[0], bulletPosition[1], 0), Quaternion.identity);
            }
            else
            {
                Instantiate(bulletPrefab, transform.position + new Vector3(-bulletPosition[0], bulletPosition[1], 0), Quaternion.identity);
            }
        }
    }

    private void UpdatePlayerMovementState()
    {
        if (_alive)
        {
            // directionX != 0 is walking/running
            if (_directionX != 0)
            {
                _playerMovementState = MovementState.Walking;
            }
            else
            {
                _playerMovementState = MovementState.Idle;
            }

            // player's y velocity indicates whether the character is jumping or falling
            if (_playerRigidbody2D.velocity.y > 0.1f)
            {
                _playerMovementState = MovementState.Jumping;
            }
            else if(_playerRigidbody2D.velocity.y < -0.1f)
            {
                _playerMovementState = MovementState.Falling;
            }
        }
        else
        {
            _playerMovementState = MovementState.Die;
        }
        
        // if the character is shooting
        if (_isFiring)
        {
            _playerMovementState = MovementState.Firing;
            // Set muzzle flame position
            if (_playerSpriteRenderer.flipX)
            {
                weapons[2].gameObject.SetActive(false);
                weapons[3].gameObject.SetActive(true);
            }
            else if(!_playerSpriteRenderer.flipX)
            {
                weapons[2].gameObject.SetActive(true);
                weapons[3].gameObject.SetActive(false);
            }
        }
        else
        {
            weapons[2].gameObject.SetActive(false);
            weapons[3].gameObject.SetActive(false);
        }

        // Update animation conditions
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
            _playerSpriteRenderer.flipX = _directionX > 0;

            // Set weapon's sprites flip
            foreach (var gunPiece in _weaponSpriteRenderers)
            {
                gunPiece.flipX = _directionX > 0;
            }
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

        SetHealthBar();
        
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

        UpdatePlayerMovementState();
        
        _gameManager.GetComponent<GameManager>().GameOver();
    }

    /// <summary>
    /// Update health bar.
    /// </summary>
    private void SetHealthBar()
    {
        switch (_health)
        {
            case 3:
                foreach (var heart in hearts)
                {
                    heart.SetActive(true);
                }
                break;
            case 2:
                hearts[2].SetActive(false);
                break;
            case 1:
                hearts[1].SetActive(false);
                hearts[2].SetActive(false);
                break;
            case 0:
                hearts[0].SetActive(false);
                hearts[1].SetActive(false);
                hearts[2].SetActive(false);
                break;
        }
    }

    /// <summary>
    /// This will be called by StepTrigger.cs when players step on monsters.
    /// </summary>
    public void StepOnMonster()
    {
        _playerRigidbody2D.velocity = new Vector2(_playerRigidbody2D.velocity.x, jumpForce/2);
    }
    
    /// <summary>
    /// enable jumping when players touch any grounds.
    /// </summary>
    public void TouchGround()
    {
        _canJump = true;
    }
}
