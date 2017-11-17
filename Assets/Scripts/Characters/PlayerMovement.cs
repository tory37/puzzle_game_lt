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

    #endregion

    public void Move(Rigidbody rigidBody, float forwardInput, float strafeInput, Vector3 forwardDirection, Vector3 rightDirection) {
        float deltaForward = forwardInput * forwardSpeed * Time.deltaTime;
        float deltaStrafe = strafeInput * strafeSpeed * Time.deltaTime;

        Vector3 deltaForwardVector = deltaForward * forwardDirection;
        Vector3 deltaStrafeVector = deltaStrafe * rightDirection;
        Vector3 deltaMovementVector = deltaForwardVector + deltaStrafeVector;

        rigidBody.MovePosition(rigidBody.position + deltaMovementVector);
    }

    public void Rotate(Transform modelTransform, Quaternion targetRotation) {
        Quaternion newRotation = Quaternion.RotateTowards(modelTransform.rotation, targetRotation, rotateSpeed);
        modelTransform.rotation = targetRotation;
    }

    public void Jump(Rigidbody rigidBody) {
        Vector3 gravity = Physics.gravity;
        rigidBody.velocity = -gravity * jumpHeight;
    }
}
