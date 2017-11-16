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
    [SerializeField]
    private Transform cameraRigY;

    //public void RotCam(Vector3 playerForward)
    //{
    //    float verticalInput = Input.GetAxis("Mouse Y");
    //    cameraRigX.Rotate(new Vector3(verticalInput * cameraVerticalSpeed * Time.deltaTime, 0f, 0f));
    //    Debug.Log(Vector3.Angle(playerForward, cameraRig.forward));
    //}

    public void RotateCamera(float horizontalInput, float verticalInput) {
        float deltaHorizontal = horizontalInput * cameraHorizontalSpeed * Time.deltaTime;
        float deltaVertical = verticalInput * cameraVerticalSpeed * Time.deltaTime;

        Quaternion oldRotation = cameraRigX.rotation;

        if (!deltaVertical.Equals(0.0f))
        {
            // Moving towards maxVerticalAngle
            if (deltaVertical > 0.0f)
            {
                Vector3 targetVectorRotation = new Vector3(maxVerticalAngle, cameraRigX.eulerAngles.y, cameraRigX.eulerAngles.z);
                Quaternion targetQuaternionRotation = Quaternion.Euler(targetVectorRotation);
                cameraRigX.rotation = Quaternion.RotateTowards(cameraRigX.rotation, targetQuaternionRotation, deltaVertical);
            }
            // Moving towards minVerticalAngle
            else if (deltaVertical < 0.0f)
            {
                Vector3 targetVectorRotation = new Vector3(minVerticalAngle, cameraRigX.eulerAngles.y, cameraRigX.eulerAngles.z);
                Quaternion targetQuaternionRotation = Quaternion.Euler(targetVectorRotation);
                cameraRigX.rotation = Quaternion.RotateTowards(cameraRigX.rotation, targetQuaternionRotation, -deltaVertical);
            }
        }
    }
}
