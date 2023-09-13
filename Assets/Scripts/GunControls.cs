using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnFiringDelegate();

public class GunControls : MonoBehaviour
{
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private SpriteRenderer weaponSpriteRenderers;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float smoothingValue;
    [SerializeField] private Vector3 bulletOffset;
    [SerializeField] private float accurcyNoise;
    [SerializeField] private AudioClip shootingSFX;
    [SerializeField] private GameObject muzzle_left;
    [SerializeField] private GameObject muzzle_right;
    
    private AudioSource audioSource;

    private bool _isFiring;
    private bool _isDoubleShotEnable;
    private Vector3 _gun2ChatacterOffset;
    private System.Random _random;
    private double _nextFireCoolDown;
    private double _nextSpecial;

    public event OnFiringDelegate OnStopFire;
    public event OnFiringDelegate OnFire;

    // Start is called before the first frame update
    void Start()
    {
        _isFiring = false;
        _isDoubleShotEnable = false;

        _gun2ChatacterOffset = transform.position - playerCharacter.gameObject.transform.position;
        _nextFireCoolDown = 0;
        _nextSpecial = 0;

        _random = new System.Random();
        audioSource = GetComponent<AudioSource>();
        
        // Make sure the gun is not visible when the game start
        SetGunVisible();

        // Debug.Log("Gun initialized!");
    }

    // Update is called once per frame
    void Update()
    {
        weaponSpriteRenderers.flipX = playerCharacter.GetComponent<SpriteRenderer>().flipX;
    }

    private void FixedUpdate()
    {
        Vector3 targetGunPos = playerCharacter.transform.position + _gun2ChatacterOffset;
        transform.position = Vector3.Lerp(transform.position, targetGunPos, smoothingValue * Time.deltaTime);
    }

    public void FireOn()
    {
        // Set a fire cooldown
        if (Time.time > _nextFireCoolDown)
        {
            _isFiring = true;
            SetGunVisible();

            SetMuzzle();

            StartCoroutine(FireBullets());
            _nextFireCoolDown = Time.time + fireRate;
        }
    }

    public void FireOff()
    {
        _isFiring = false;
        SetGunVisible();
        
        SetMuzzle();
    }

    private void SetMuzzle()
    {
        if (weaponSpriteRenderers.flipX)
        {
            muzzle_right.SetActive(_isFiring);
        }
        else
        {
            muzzle_left.SetActive(_isFiring);
        }
    }

    private void SetGunVisible()
    {
        // Make the gun visible
        weaponSpriteRenderers.gameObject.SetActive(_isFiring);
    }

    IEnumerator FireBullets()
    {
        // While the player is firing
        while (_isFiring)
        {
            SpawnBullet();
            yield return new WaitForSeconds(fireRate); // Wait for the fire rate duration before spawning the next bullet
        }

        // To fix "double click" issue
        OnStopFire?.Invoke();
    }
    
    // Call this method to spawn a bullet
    private void SpawnBullet()
    {
        // OnFire recoil
        playerCharacter.GetComponent<PlayerControls>().FireRecoil();
        
        // Play shooting soundFX
        audioSource.PlayOneShot(shootingSFX);
        
        // Instantiate the bullet at the spawn point
        var position = transform.position;
        Vector3 bulletSpawnPosition;
        if (weaponSpriteRenderers.flipX)
        {
            bulletSpawnPosition = position + bulletOffset;
        }
        else
        {
            bulletSpawnPosition = new Vector3(position.x - bulletOffset.x, position.y + bulletOffset.y, position.z);
        }
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);

        // Get the Rigidbody2D component of the bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        

        // If the player is facing right, shoot the bullet to the right
        if (weaponSpriteRenderers.flipX)
        {
            rb.velocity = new Vector2(bulletSpeed, ((float)_random.NextDouble()-0.3f)*accurcyNoise);
        }
        // If the player is facing left, shoot the bullet to the left
        else
        {
            rb.velocity = new Vector2(-bulletSpeed, ((float)_random.NextDouble()-0.3f)*accurcyNoise);
        }
    }

    public void EnableDoubleShot()
    {
        if (!_isDoubleShotEnable && Time.time > _nextSpecial)
        {
            _nextSpecial = Time.time + 6f;
            
            _isDoubleShotEnable = true;
            StartCoroutine(DisableDoubleShot());
        }
    }

    IEnumerator DisableDoubleShot()
    {
        yield return new WaitForSeconds(3f);
        _isDoubleShotEnable = false;
    }
}
