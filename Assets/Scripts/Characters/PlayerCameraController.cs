using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

    [SerializeField]
    private float cameraHorizontalSpeed = 5.0f;
    [SerializeField]
    private float cameraVerticalSpeed = 5.0f;

    [SerializeField]
    private float minVerticalAngle;
    [SerializeField]
    private float maxVerticalAngle;

    [SerializeField]
    private Transform cameraRigX;
    private Transform cameraRigY;

    public void RotCam(Vector3 playerForward)
    {
        float verticalInput = Input.GetAxis("Mouse Y");
        cameraRig.Rotate(new Vector3(verticalInput * cameraVerticalSpeed * Time.deltaTime, 0f, 0f));
        Debug.Log(Vector3.Angle(playerForward, cameraRig.forward));
    }

    public void RotateCamera(float horiontalInput, float verticalInput) {
        float deltaHorizontal = horiontalInput * cameraHorizontalSpeed * Time.deltaTime;
        float deltaVertical = verticalInput * cameraVerticalSpeed * Time.deltaTime;

        Vector3 oldRotation = cameraRig.rotation.eulerAngles;

        float oldRotationX = oldRotation.x;
        float newRotationX = oldRotationX + deltaVertical;

        if (verticalInput > 0) {
            //if (Mathf.Abs(0 - ))
        }
    }
}
