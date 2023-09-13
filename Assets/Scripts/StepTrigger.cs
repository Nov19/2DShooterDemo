using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StepTriggerDelegate();

public class StepTrigger : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObject;

    public event StepTriggerDelegate OnStepEnter;
    public event StepTriggerDelegate OnStepExit;

    private PlayerControls _playerScript;
    private BoxCollider2D _stepTriggerCollider2D;

    // Start is called before the first frame update
    void Start()
    {
        _playerScript = playerGameObject.GetComponent<PlayerControls>();
        _stepTriggerCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_stepTriggerCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemies")))
        {
            OnStepExit?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _playerScript.TouchGround();
        
        if (other.CompareTag("Dmg2Player"))
        {
            _playerScript.StepOnMonster();
            OnStepEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Dmg2Player"))
        {
            OnStepExit?.Invoke();
        }
    }
}
