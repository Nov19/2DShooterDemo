using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControls : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Terrain") || other.CompareTag("Dmg2Player"))
        {
            // TODO: Hit VFX - hit a wall / enemies
            var explosionVFX = Instantiate(explosionPrefab, transform.position, transform.rotation);
        
            Destroy(explosionVFX, 0.12f);

            // Destroy the bullet after spawning the VFX
            Destroy(gameObject);
        }
    }
}
