using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{

    #region Variables

    [Header("Components")]
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Transform cameraRigX;
    [SerializeField]
    private Transform cameraRigY;

    [Header("Horizontal Rotation")]
    [SerializeField]
    private float cameraHorizontalSpeed = 5.0f;

    [Header("Vertical Rotation")]
    [SerializeField]
    private float cameraVerticalSpeed = 5.0f;
    [SerializeField]
    private float minVerticalAngle;
    [SerializeField]
    private float maxVerticalAngle;

    [Header("Zoom")]
    [SerializeField]
    private float cameraZoomSpeed = 5.0f;
    [SerializeField]
    private Vector3 closestCameraOffset;
    [SerializeField]
    private Vector3 farthestCameraOffset;

    [Header("Collision")]
    [SerializeField]
    private float collisionCheckRaycastLength;
    [SerializeField]
    private float collisionCheckRaycastDeadzone;

    private Vector3 storedCameraLocalPosition;

    #endregion

    #region Methods

    private void Start() {
        storedCameraLocalPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        RotateCamera();
        ZoomCamera();
        cameraTransform.localPosition = storedCameraLocalPosition;
    }

    public void RotateCamera()
    {
        float verticalInput = Input.GetAxis(InputNames.Mouse_Y);
        float horizontalInput = Input.GetAxis(InputNames.Mouse_X);

        Quaternion oldRotation = cameraRigX.rotation;

        // Rotate Vertical
        if (!verticalInput.Equals(0.0f))
        {
            float deltaVertical = verticalInput * cameraVerticalSpeed * Time.deltaTime;

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

        // Rotate Horizontal
        if (!horizontalInput.Equals(0.0f))
        {
            float deltaHorizontal = horizontalInput * cameraHorizontalSpeed * Time.deltaTime;

            Vector3 deltaVectorRotation = cameraRigY.up * deltaHorizontal;
            cameraRigY.Rotate(deltaVectorRotation);
        }
    }

    public void ZoomCamera()
    {
        float zoomInput = Input.GetAxis(InputNames.Scroll);

        if (!zoomInput.Equals(0.0f))
        {
            float deltaZoom = zoomInput * cameraZoomSpeed * Time.deltaTime;
            Debug.Log(deltaZoom);

            // Moving towards maxVerticalAngle
            if (deltaZoom > 0.0f)
            {
                storedCameraLocalPosition = Vector3.MoveTowards(cameraTransform.localPosition, closestCameraOffset, deltaZoom);
            }
            // Moving towards minVerticalAngle
            else if (deltaZoom < 0.0f)
            {
                storedCameraLocalPosition = Vector3.MoveTowards(cameraTransform.localPosition, farthestCameraOffset, -deltaZoom);
            }
        }
    }

    public Transform GetCameraRigYTransform()
    {
        return cameraRigY;
    }

    private void CheckCollision() {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, -cameraTransform.forward, out hit, collisionCheckRaycastLength )) {
            
        }
    }

    #endregion
}
