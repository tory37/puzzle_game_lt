using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{

    #region Variables

    [Header("Components")]
    [SerializeField]
    [Tooltip("Transform of the camera for this controller")]
    private Transform cameraTransform;
    [SerializeField]
    private Transform cameraRigX;
    [SerializeField]
    private Transform cameraRigY;

    [Header("Horizontal Rotation")]
    [SerializeField]
    [Tooltip("Camera Horizontal Speed")]
    private float cameraHorizontalSpeed = 5.0f;

    [Header("Vertical Rotation")]
    [SerializeField]
    [Tooltip("Camera Vertical Speed")]
    private float cameraVerticalSpeed = 5.0f;
    [SerializeField]
    private float minVerticalAngle;
    [SerializeField]
    private float maxVerticalAngle;

    [Header("Zoom")]
    [SerializeField]
    private float cameraZoomSpeed = 5.0f;
    [SerializeField]
    [Tooltip("Farthest Camera Offset")]
    private Vector3 farthestCameraOffset;

    [Header("Collision")]
    [SerializeField]
    [Tooltip("Collision Check Raycast Length")]
    private float collisionCheckRaycastLength;
    [SerializeField]
    [Tooltip("Collision Check Spherecast Radius")]
    private float collisionCheckRaycastRadius;
    [SerializeField]
    [Tooltip("Closest Distance Camera Can Be To Objects It Shouldn't Clip")]
    private float closestDistanceToCollidables;
    [SerializeField]
    [Tooltip("Camera Adjustment Speed")]
    private float cameraAdjustmentSpeed;

    private Vector3 userTargetCameraPosition;
    private Vector3 targetCameraLocalPosition;

    #endregion

    #region Methods

    private void Start()
    {
        targetCameraLocalPosition = cameraTransform.position;
        userTargetCameraPosition = cameraTransform.position;
        collisionCheckRaycastLength = closestDistanceToCollidables * 5;
        cameraTransform.localPosition = farthestCameraOffset;
    }

    private void LateUpdate()
    {
        RotateCamera();
        ZoomCamera();
        MoveCamera();
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

            // Moving towards maxVerticalAngle
            if (deltaZoom > 0.0f)
            {
                userTargetCameraPosition = Vector3.MoveTowards(userTargetCameraPosition, Vector3.zero, deltaZoom);
            }
            // Moving towards minVerticalAngle
            else if (deltaZoom < 0.0f)
            {
                userTargetCameraPosition = Vector3.MoveTowards(userTargetCameraPosition, farthestCameraOffset, -deltaZoom);
            }
        }
    }

    public Transform GetCameraRigYTransform()
    {
        return cameraRigY;
    }

    private Vector3 GetAdjustedCollidingCameraTargetPosition(RaycastHit hit)
    {
        Vector3 hitLocalToRig = cameraRigX.InverseTransformPoint(hit.point);
        Vector3 direction = hitLocalToRig.normalized;
        float hitDistance = hitLocalToRig.magnitude;
        Vector3 targetLocalPosition = direction * (hitDistance - 0.05f);
        return targetLocalPosition;
    }

    private void MoveCamera()
    {
        SetTargetPosition();
        cameraTransform.localPosition = Vector3.MoveTowards(cameraTransform.localPosition, targetCameraLocalPosition, cameraAdjustmentSpeed * Time.deltaTime);
    }

    private void SetTargetPosition() 
    {
        float cameraDistance = cameraTransform.localPosition.magnitude;
        
#if UNITY_EDITOR
        Vector3 rigPosition = cameraRigY.position;
        Debug.DrawRay(rigPosition, (cameraTransform.position - cameraRigX.transform.position).normalized * farthestCameraOffset.magnitude, Color.green);
#endif

        RaycastHit hit;
        bool bIsHitting = Physics.Raycast(cameraRigY.position, (cameraTransform.position - cameraRigX.transform.position).normalized, out hit, farthestCameraOffset.magnitude);
        if (bIsHitting) {
            Debug.DrawRay(hit.point, Vector3.up * 10, Color.black);
            MultiTagSystem tags = hit.transform.GetComponent<MultiTagSystem>();
            if (tags && tags.HasTag(MultiTag.MoveCameraOnCollision))
            {
                if (userTargetCameraPosition.magnitude > cameraRigY.InverseTransformPoint(hit.point).magnitude)
                {
                    targetCameraLocalPosition = GetAdjustedCollidingCameraTargetPosition(hit);
                    return;
                }
            }
        }

        //Else
        targetCameraLocalPosition = userTargetCameraPosition;
    }

#endregion
}
