using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraControls : MonoBehaviour
{
    [FormerlySerializedAs("obj2Follow")] [SerializeField] private GameObject playerGameObject;
    [SerializeField] private float cameraHight;
    [SerializeField] private float smoothingValue = 5.0f;
    [SerializeField] private float lookAHeadDistance = 20.0f;

    private Vector3 camera2CharacterOffset;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize camera position
        Vector3 newPosition = playerGameObject.transform.position;
        transform.position = new Vector3(newPosition.x, newPosition.y + cameraHight, this.transform.position.z);
        
        camera2CharacterOffset = transform.position - newPosition;

        // Debug.Log("Camera controls initialized!");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 targetCamPos;
        if (playerGameObject.GetComponent<PlayerControls>().isFacingRight()) // Assuming your player's script is named "PlayerScript"
        {
            targetCamPos = playerGameObject.transform.position + camera2CharacterOffset + new Vector3(lookAHeadDistance, 0, 0);
        }
        else
        {
            targetCamPos = playerGameObject.transform.position + camera2CharacterOffset - new Vector3(lookAHeadDistance, 0, 0);
        }
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothingValue * Time.deltaTime);
    }
}
