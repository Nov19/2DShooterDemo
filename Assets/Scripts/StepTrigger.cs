using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepTrigger : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObject;

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
        if (other.CompareTag("Dmg2Player"))
        {
            playerGameObject.GetComponent<PlayerControls>().StepOnMonster();
        }
        else
        {
            playerGameObject.GetComponent<PlayerControls>().TouchGround();
        }
    }
}
