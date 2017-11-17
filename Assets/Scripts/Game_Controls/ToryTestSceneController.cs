using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToryTestSceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
	}
}
