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
}
