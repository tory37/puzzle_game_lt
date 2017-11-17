﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Components
    [Header("Components")]
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private PlayerCameraController cameraController;

    [SerializeField]
    private Rigidbody playerRigidBody;

    [SerializeField]
    private Transform modelTransform;

    private Transform playerTransform;

    #endregion

    #region Variables 
    [Header("Jump Raycast")]
    [SerializeField]
    private Vector3 jumpRaycastOffset;
    [SerializeField]
    private float jumpRaycastDistance;
    [SerializeField]
    private LayerMask jumpRaycastLayerMask;

    #endregion

    // Use this for initialization
    void Start()
    {
        playerTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
        JumpPlayer();
        RotateCamera();
        ZoomCamera();
    }

    private void MovePlayer()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float strafeInput = Input.GetAxis("Horizontal");
        Transform cameraRigYTransform = cameraController.GetCameraRigYTransform();
        Vector3 forwardDirection = cameraRigYTransform.forward;
        Vector3 rightDirection = cameraRigYTransform.right;
        playerMovement.Move(playerRigidBody, forwardInput, strafeInput, forwardDirection, rightDirection);
    }

    private void RotatePlayer()
    {
        bool rightMouse = Input.GetButton("Mouse Right");
        float forwardInput = Input.GetAxis("Vertical");
        float strafeInput = Input.GetAxis("Horizontal");
        if (rightMouse || !forwardInput.Equals(0.0f) || !strafeInput.Equals(0.0f))
        {
            float horizontalInput = Input.GetAxis("Mouse X");
            playerMovement.Rotate(modelTransform, cameraController.GetCameraRigYTransform().rotation);
        }
    }

    private void JumpPlayer()
    {
        #if UNITY_EDITOR
        Debug.DrawRay(playerTransform.position + jumpRaycastOffset, -playerTransform.up * jumpRaycastDistance, Color.red);
        #endif

        if (CanJump())
        {
            bool jumpInput = Input.GetButtonDown("Jump");
            if (jumpInput)
            {
                playerMovement.Jump(playerRigidBody);
            }
        }
    }

    private bool CanJump() {
        RaycastHit raycastHit;
        if (Physics.Raycast(playerTransform.position + jumpRaycastOffset, -playerTransform.up, out raycastHit, jumpRaycastDistance, jumpRaycastLayerMask )) {
            Collider otherCollider = raycastHit.collider;
            MultiTagSystem otherTags = otherCollider.GetComponent<MultiTagSystem>();
            if (otherTags)
            {
                if (otherTags.HasTag(MultiTag.Jumpable))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void RotateCamera() {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");
        cameraController.RotateCamera(horizontalInput, verticalInput);
    }

    private void ZoomCamera() {
        float zoomInput = Input.GetAxis("Scroll");
        cameraController.ZoomCamera(zoomInput);
    }
}
