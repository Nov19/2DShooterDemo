using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MonsterSpawner : MonoBehaviour
{
    // A roughly cooldown
    [SerializeField] private float spawnCoolDown;
    [SerializeField] private GameObject spawnSign;
    [SerializeField] private GameObject monster2Spawn;

    private float _spawnTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnTimer = spawnCoolDown;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _spawnTimer)
        {
            _spawnTimer += UnityEngine.Random.Range(spawnCoolDown, spawnCoolDown*2);

            // Mimic the power charge to spawn the monster
            spawnSign.gameObject.SetActive(true);
            
            Invoke(nameof(SpawnAMonster), 2f);
        }
    }

    private void SpawnAMonster()
    {
        Vector3 position = transform.position;
        Vector3 spawnPosition = new Vector3(position.x, position.y+0.1f, position.z);
        Instantiate(monster2Spawn, position, Quaternion.identity);
        
        // Mimic the power charge to spawn the monster
        spawnSign.gameObject.SetActive(false);
    }
}
