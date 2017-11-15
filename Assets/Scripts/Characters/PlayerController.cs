using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Components

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private Rigidbody playerRigidBody;

    private Transform playerTransform;

    #endregion

    // Use this for initialization
    void Start () {
        playerTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        MovePlayer();
        RotatePlayer();
        JumpPlayer();
	}

    private void MovePlayer() {
        float forwardInput = Input.GetAxis("Vertical");
        float strafeInput = Input.GetAxis("Horizontal");
        playerMovement.Move(playerTransform, playerRigidBody, forwardInput, strafeInput);
    }

    private void RotatePlayer() {
        float horizontalInput = Input.GetAxis("Mouse X");
        playerMovement.Rotate(playerRigidBody, horizontalInput);
    }

    private void JumpPlayer() {
        bool jumpInput = Input.GetButtonDown("Jump");
        if (jumpInput)
        {
            playerMovement.Jump(playerRigidBody);
        }
    }
}
