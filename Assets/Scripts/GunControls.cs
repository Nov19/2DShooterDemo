using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StopFiringDelegate();

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
    
    private AudioSource audioSource;

    private bool _isFiring;
    private bool _isFiringCoroutineRunning;
    private Vector3 _gun2ChatacterOffset;
    private System.Random random;
    private double nextFireCoolDown;

    public event StopFiringDelegate OnStopFire;
    
    // Start is called before the first frame update
    void Start()
    {
        _isFiring = false;
        _isFiringCoroutineRunning = false;
        
        _gun2ChatacterOffset = transform.position - playerCharacter.gameObject.transform.position;
        nextFireCoolDown = 0;

        random = new System.Random();
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
        if (Time.time > nextFireCoolDown)
        {
            _isFiring = true;
            SetGunVisible();

            StartCoroutine(FireBullets());
            nextFireCoolDown = Time.time + fireRate;
        }
    }

    public void FireOff()
    {
        _isFiring = false;
        SetGunVisible();
    }
    
    private void SetGunVisible()
    {
        // Make the gun visible
        weaponSpriteRenderers.gameObject.SetActive(_isFiring);
    }

    IEnumerator FireBullets()
    {
        // To fix "double click" issue
        _isFiringCoroutineRunning = true;
        
        // While the player is firing
        while (_isFiring)
        {
            SpawnBullet();
            yield return new WaitForSeconds(fireRate); // Wait for the fire rate duration before spawning the next bullet
        }

        // To fix "double click" issue
        OnStopFire?.Invoke();
        _isFiringCoroutineRunning = false;
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
            rb.velocity = new Vector2(bulletSpeed, ((float)random.NextDouble()-0.3f)*accurcyNoise);
        }
        // If the player is facing left, shoot the bullet to the left
        else
        {
            rb.velocity = new Vector2(-bulletSpeed, ((float)random.NextDouble()-0.3f)*accurcyNoise);
        }
    }

    public bool isFiring()
    {
        return _isFiring;
    }
}
