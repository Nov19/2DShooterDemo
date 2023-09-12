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
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float[] bulletPosition;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float fireRate;

    // Main character's dynamic status
    private bool _alive;
    private bool _canJump;
    private int _health;
    private float _directionX;
    private bool _isFiring;
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
            // If the fire button is pressed and the player is not currently firing
            if (Input.GetButtonDown("Fire1") && !_isFiring)
            {
                _isFiring = true;
                SetGunVisible();
                StartCoroutine(FireBullets());
            }

            // If the fire button is released
            if (Input.GetButtonUp("Fire1"))
            {
                _isFiring = false;
                SetGunVisible();
            }
        }
    }

    private void SetGunVisible()
    {
        // Make the gun visible
        for(int i = 0; i < _weaponSpriteRenderers.Length; i++)
        {
            weapons[i].gameObject.SetActive(_isFiring);
        }
    }

    IEnumerator FireBullets()
    {
        // While the player is firing
        while (_isFiring)
        {
            SpawnBullet();
            yield return new WaitForSeconds(fireRate); // Wait for the fire rate duration before spawning the next bullet
        }
    }
    
    // Call this method to spawn a bullet
    private void SpawnBullet()
    {
        // Instantiate the bullet at the spawn point
        GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(-bulletPosition[0], bulletPosition[1], 0), Quaternion.identity);

        // Get the Rigidbody2D component of the bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        // If the player is facing right, shoot the bullet to the right
        if (_isFacingRight)
        {
            rb.velocity = new Vector2(bulletSpeed, 0);
        }
        // If the player is facing left, shoot the bullet to the left
        else
        {
            rb.velocity = new Vector2(-bulletSpeed, 0);
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

            // Set weapon's sprites flip
            foreach (var gunPiece in _weaponSpriteRenderers)
            {
                gunPiece.flipX = _isFacingRight;
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

        // UpdatePlayerMovementState();
        
        _gameManager.GetComponent<GameManager>().GameOver();
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

    public bool isFacingRight()
    {
        return _isFacingRight;
    }
}
