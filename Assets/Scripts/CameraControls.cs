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
    private PlayerControls _playerScript;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Initialize camera position
        Vector3 playerPosition = playerGameObject.transform.position;

        transform.position = new Vector3(playerPosition.x, playerPosition.y + cameraHight, this.transform.position.z);
        camera2CharacterOffset = transform.position - playerPosition;

        _playerScript = playerGameObject.GetComponent<PlayerControls>();

        Debug.Log("Camera controls initialized!");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 playerCamPos;
        if (_playerScript.isFacingRight()) // Assuming your player's script is named "PlayerScript"
        {
            playerCamPos = playerGameObject.transform.position + camera2CharacterOffset + new Vector3(lookAHeadDistance, 0, 0);
        }
        else
        {
            playerCamPos = playerGameObject.transform.position + camera2CharacterOffset - new Vector3(lookAHeadDistance, 0, 0);
        }
        transform.position = Vector3.Lerp(transform.position, playerCamPos, smoothingValue * Time.deltaTime);
    }
}
