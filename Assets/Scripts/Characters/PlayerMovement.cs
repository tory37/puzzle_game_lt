using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    #region Variables

    [SerializeField]
    private float forwardSpeed = 5.0f;
    [SerializeField]
    private float strafeSpeed = 5.0f;

    #endregion

    public void Move(Rigidbody rigidBody, float forwardInput, float strafeInput) {
        float deltaForward = forwardInput * forwardSpeed * Time.deltaTime;
        float deltaStrafe = strafeInput * strafeSpeed * Time.deltaTime;
        Vector3 deltaMovement = new Vector3(deltaStrafe, 0.0f, deltaForward);
        rigidBody.MovePosition(rigidBody.position + deltaMovement);
    }
}
