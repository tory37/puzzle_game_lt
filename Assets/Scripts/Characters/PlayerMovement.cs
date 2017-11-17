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

    [Header("Components")]
    [SerializeField]
    private Transform targetRotationTransform;
    [SerializeField]
    private Transform modelTransform;

    private Rigidbody targetRigidbody;

    #endregion

    private void Start() {
        targetRigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        Move();
        Rotate();
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

    public void Jump(Rigidbody rigidBody) {
        Vector3 gravity = Physics.gravity;
        rigidBody.velocity = -gravity * jumpHeight;
    }

    private bool ShouldRotateModel() {
        bool bLookButtonHeld = Input.GetButton(InputNames.Mouse_Right);
        bool bMovingForward = !Input.GetAxis(InputNames.Forward).Equals(0.0f);
        bool bStrafing = !Input.GetAxis(InputNames.Strafe).Equals(0.0f);

        return bLookButtonHeld || bMovingForward || bStrafing;
    }
}
