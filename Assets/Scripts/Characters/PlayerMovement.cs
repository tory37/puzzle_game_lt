using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Variables
    [Header("Move")]
    [SerializeField]
    private float forwardSpeed = 5.0f;
    [SerializeField]
    private float strafeSpeed = 5.0f;

    [Header("Rotate")]
    [SerializeField]
    private float rotateSpeed = 100.0f;

    [Header("Jump")]
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private Vector3 jumpRaycastOffset;
    [SerializeField]
    private float jumpRaycastDistance;
    [SerializeField]
    private LayerMask jumpRaycastLayerMask;

    [Header("Components")]
    [SerializeField]
    private Transform targetRotationTransform;
    [SerializeField]
    private Transform modelTransform;

    private Rigidbody targetRigidbody;

    #endregion

    private void Start() {
        targetRigidbody = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }

    private void FixedUpdate() {
        Move();
        Rotate();
        Jump();
    }

    public void Move() {
        float forwardInput = Input.GetAxis(InputNames.Forward);
        float strafeInput = Input.GetAxis(InputNames.Strafe);

        float deltaForward = forwardInput * forwardSpeed * Time.deltaTime;
        float deltaStrafe = strafeInput * strafeSpeed * Time.deltaTime;

        Vector3 deltaForwardVector = deltaForward * targetRotationTransform.forward;
        Vector3 deltaStrafeVector = deltaStrafe * targetRotationTransform.right;
        Vector3 deltaMovementVector = deltaForwardVector + deltaStrafeVector;

        targetRigidbody.MovePosition(targetRigidbody.position + deltaMovementVector);
    }

    public void Rotate() {
        if (ShouldRotateModel())
        {
            Quaternion targetRotation = targetRotationTransform.rotation;
            Quaternion newRotation = Quaternion.RotateTowards(modelTransform.rotation, targetRotation, rotateSpeed);
            modelTransform.rotation = targetRotation;
        }
    }

    public void Jump() {
        #if UNITY_EDITOR
        Debug.DrawRay(targetRigidbody.position + jumpRaycastOffset, -targetRigidbody.transform.up * jumpRaycastDistance, Color.red);
        #endif

        if (CanJump())
        {
            bool jumpInput = Input.GetButtonDown("Jump");
            if (jumpInput)
            {
                Vector3 gravity = Physics.gravity;
                targetRigidbody.velocity = -gravity * jumpHeight;
            }
        }
    }

    private bool ShouldRotateModel() {
        bool bLookButtonHeld = Input.GetButton(InputNames.Mouse_Right);
        bool bMovingForward = !Input.GetAxis(InputNames.Forward).Equals(0.0f);
        bool bStrafing = !Input.GetAxis(InputNames.Strafe).Equals(0.0f);

        return bLookButtonHeld || bMovingForward || bStrafing;
    }

    private bool CanJump()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(targetRigidbody.position + jumpRaycastOffset, -targetRigidbody.transform.up, out raycastHit, jumpRaycastDistance, jumpRaycastLayerMask))
        {
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
