using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ThirdPersonCameraTakeControlCallback();
public delegate void ThirdPersonCameraReleaseControlCallback();

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
    [SerializeField]
    private CameraTrigger cameraTrigger;
    [SerializeField]
    private Renderer playerRenderer;

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
    [Tooltip("Camera Adjustment Speed")]
    private float cameraAdjustmentSpeed;
    [SerializeField]
    private float cameraPlayerRendererToggleDistance;

    private Vector3 userTargetCameraPosition;
    private Vector3 targetCameraLocalPosition;

    private MonoBehaviour controllingBehaviour = null;
    private bool rotationEnabled;
    private bool zoomEnabled;
    private Vector3? controlledZoomLocalPosition = null;

    #endregion

    #region Methods

    private void Start()
    {
        targetCameraLocalPosition = cameraTransform.position;
        userTargetCameraPosition = cameraTransform.position;
        cameraTransform.localPosition = farthestCameraOffset;

        rotationEnabled = true;
        zoomEnabled = true;

        cameraTrigger.Subscribe(HandleOnCameraTriggerEnter, HandleOnCameraTriggerExit);
    }

    private void LateUpdate()
    {
        RotateCamera();
        ZoomCamera();
        MoveCamera();
    }

    // TODO Move actual rotation to MoveCamera function, set a variable that the camera rigs rotate toward
    public void RotateCamera()
    {
        if (rotationEnabled)
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
    }

    public void ZoomCamera()
    {
        if (zoomEnabled)
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
        Vector3 targetLocalPosition = Vector3.MoveTowards(cameraTransform.localPosition, targetCameraLocalPosition, cameraAdjustmentSpeed * Time.deltaTime);
        if (targetLocalPosition != cameraTransform.localPosition)
        {
            cameraTransform.localPosition = targetLocalPosition;
            CheckPlayerCameraDistance();
        }
    }

    private void CheckPlayerCameraDistance()
    {
        if (cameraTransform.localPosition.magnitude <= cameraPlayerRendererToggleDistance)
        {
            playerRenderer.enabled = false;
        }
        else
        {
            playerRenderer.enabled = true;
        }
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
        if (bIsHitting)
        {
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

        if (controlledZoomLocalPosition != null)
        {
            targetCameraLocalPosition = (Vector3)controlledZoomLocalPosition;
        }
        else
        {
            targetCameraLocalPosition = userTargetCameraPosition;
        }
    }

    private bool IsControlling(MonoBehaviour caller)
    {
        return caller == controllingBehaviour;
    }

    public bool AttemptTakeControl(MonoBehaviour caller, ThirdPersonCameraTakeControlCallback onSuccess = null, ThirdPersonCameraTakeControlCallback onFailure = null)
    {
        if (controllingBehaviour == null)
        {
            controllingBehaviour = caller;
            if (onSuccess != null)
                onSuccess();
            return true;
        }
        else
        {
            if (onFailure != null)
                onFailure();
            return false;
        }
    }

    public bool AttemptReleaseControl(MonoBehaviour caller, ThirdPersonCameraReleaseControlCallback onSuccess = null, ThirdPersonCameraReleaseControlCallback onFailure = null)
    {
        if (controllingBehaviour == caller)
        {
            AttemptEnableCameraRotation(caller);
            AttemptEnableCameraZoom(caller);
            controllingBehaviour = null;
            controlledZoomLocalPosition = null;
            if (onSuccess != null)
                onSuccess();
            return true;
        }
        else
        {
            if (onFailure != null)
                onFailure();
            return false;
        }
    }

    /// <summary>
    /// Gets the camera transform.  Please do not fuck with the camera. Thank you.
    /// </summary>
    /// <returns>The camera transform.</returns>
    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public bool AttemptDisableCameraRotation(MonoBehaviour caller)
    {
        if (IsControlling(caller))
        {
            rotationEnabled = false;
            return true;
        }
        return false;
    }

    public bool AttemptEnableCameraRotation(MonoBehaviour caller)
    {
        if (IsControlling(caller))
        {
            rotationEnabled = true;
            return true;
        }
        return false;
    }

    public bool AttemptDisableCameraZoom(MonoBehaviour caller)
    {
        if (IsControlling(caller))
        {
            zoomEnabled = false;
            return true;
        }
        return false;
    }

    public bool AttemptEnableCameraZoom(MonoBehaviour caller)
    {
        if (IsControlling(caller))
        {
            zoomEnabled = true;
            return true;
        }
        return false;
    }


    // TODO Implement this
    public bool SetTargetRotation(Quaternion Rotation, MonoBehaviour caller)
    {

        return false;
    }

    public bool SetTargetZoom(float zoomDistance, MonoBehaviour caller)
    {
        if (IsControlling(caller))
        {
            Vector3 direction = (Vector3.zero - farthestCameraOffset).normalized;
            controlledZoomLocalPosition = direction * zoomDistance;
            return true;
        }
        return false;
    }

    private void HandleOnCameraTriggerEnter(Collider enteredCollider)
    {
        enteredCollider.GetComponent<Renderer>().enabled = false;
    }

    private void HandleOnCameraTriggerExit(Collider exitedCollider)
    {
        exitedCollider.GetComponent<Renderer>().enabled = true;
    }

    #endregion
}
