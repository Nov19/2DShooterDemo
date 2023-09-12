using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BulletControls : MonoBehaviour
{
    // The larger, the faster
    [SerializeField] private float bulletSpeed;
    // The larger, the less accurate
    [SerializeField] private float bulletDeflection;

    private Rigidbody2D _bulletRigidbody2D;
    private BoxCollider2D _bulletBoxCollider2D;
    private SpriteRenderer _bulletSpriteRenderer;
    private SpriteRenderer _playerSpriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        _bulletRigidbody2D = GetComponent<Rigidbody2D>();
        _bulletBoxCollider2D = GetComponent<BoxCollider2D>();
        _bulletSpriteRenderer = GetComponent<SpriteRenderer>();
        _playerSpriteRenderer = GameObject.Find("Player").GetComponent<SpriteRenderer>();

        // Set bullet's sprite flipX
        _bulletSpriteRenderer.flipX = _playerSpriteRenderer.flipX;
        
        // FlipX is positive, velocity should be point to the right
        if (_playerSpriteRenderer.flipX)
        {
            _bulletRigidbody2D.velocity = new Vector2(bulletSpeed, Random.Range(-bulletDeflection, bulletDeflection));
        }
        else
        {
            _bulletRigidbody2D.velocity = new Vector2(-bulletSpeed, Random.Range(-bulletDeflection, bulletDeflection));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
