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

    public void Move(Transform transform, Rigidbody rigidBody, float forwardInput, float strafeInput) {
        float deltaForward = forwardInput * forwardSpeed * Time.deltaTime;
        float deltaStrafe = strafeInput * strafeSpeed * Time.deltaTime;

        Vector3 deltaForwardVector = deltaForward * transform.forward;
        Vector3 deltaStrafeVector = deltaStrafe * transform.right;
        Vector3 deltaMovementVector = deltaForwardVector + deltaStrafeVector;

        rigidBody.MovePosition(rigidBody.position + deltaMovementVector);
    }

    public void Rotate(Rigidbody rigidBody, float horizontalInput) {
        float deltaHorizontal = horizontalInput * rotateSpeed * Time.deltaTime;
        Vector3 deltaVectorRotation = new Vector3(0.0f, deltaHorizontal, 0.0f);
        Quaternion deltaQuaternionRotation = Quaternion.Euler(deltaVectorRotation);
        rigidBody.MoveRotation(rigidBody.rotation * deltaQuaternionRotation);
    }

    public void Jump(Rigidbody rigidBody) {
        Vector3 gravity = Physics.gravity;
        rigidBody.velocity = -gravity * jumpHeight;
    }
}
