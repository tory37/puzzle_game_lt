using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Components
    [Header("Components")]
    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private Rigidbody playerRigidBody;

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
    }

    private void MovePlayer()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float strafeInput = Input.GetAxis("Horizontal");
        playerMovement.Move(playerTransform, playerRigidBody, forwardInput, strafeInput);
    }

    private void RotatePlayer()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        playerMovement.Rotate(playerRigidBody, horizontalInput);
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
}
