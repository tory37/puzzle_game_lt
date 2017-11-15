using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Components

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private Rigidbody rigidBody;

    #endregion

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float forwardInput = Input.GetAxis("Vertical");
        float strafeInput = Input.GetAxis("Horizontal");
        playerMovement.Move(rigidBody, forwardInput, strafeInput);
	}
}
