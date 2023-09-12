using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool _isFiring;
    private Vector3 _gun2ChatacterOffset;
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        _gun2ChatacterOffset = transform.position - playerCharacter.gameObject.transform.position;
        random = new System.Random();
        
        Debug.Log("Gun initialized!");
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
        _isFiring = true;
        SetGunVisible();
        StartCoroutine(FireBullets());
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
        // OnFire recoil
        playerCharacter.GetComponent<PlayerControls>().FireRecoil();
        
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
}
