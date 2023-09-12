using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GunControls : MonoBehaviour
{
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private SpriteRenderer[] weaponSpriteRenderers;
    [SerializeField] private float fireRate;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float smoothingValue;


    private PlayerControls _playerScript;
    private Vector3 _playerPosition;
    private Vector3 _gun2PlayerOffset;

    // Start is called before the first frame update
    void Start()
    {
        _playerScript = playerCharacter.GetComponent<PlayerControls>();
        _playerPosition = playerCharacter.transform.position;

        _gun2PlayerOffset = transform.position - _playerPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newGunPos = _playerPosition + _gun2PlayerOffset;
        transform.position = Vector3.Lerp(transform.position, newGunPos, smoothingValue * Time.deltaTime);
    }
    
    private void SetGunVisible()
    {
        // Make the gun visible
        foreach (var renderer in weaponSpriteRenderers)
        {
            renderer.gameObject.SetActive(_playerScript.isFiring());
        }
    }

    IEnumerator FireBullets()
    {
        // While the player is firing
        while (_playerScript.isFiring())
        {
            SpawnBullet();
            yield return new WaitForSeconds(fireRate); // Wait for the fire rate duration before spawning the next bullet
        }
    }
    
    // Call this method to spawn a bullet
    private void SpawnBullet()
    {
        // Instantiate the bullet at the spawn point
        GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(transform.position.x, transform.position.y), Quaternion.identity);

        // Get the Rigidbody2D component of the bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        // If the player is facing right, shoot the bullet to the right
        if (_playerScript.isFacingRight())
        {
            rb.velocity = new Vector2(bulletSpeed, 0);
        }
        // If the player is facing left, shoot the bullet to the left
        else
        {
            rb.velocity = new Vector2(-bulletSpeed, 0);
        }
    }
}