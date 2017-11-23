using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : PlayerAbility
{
    [Header("Components")]
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private ThirdPersonCameraController cameraController;
    [SerializeField]
    private GameObject targetLocationMarkerPrefab;
    [SerializeField]
    private Rigidbody playerRigidbody;

    [Header("Variables")]
    [SerializeField]
    private float minTargetDistance;
    [SerializeField]
    private float maxTargetDistance;
    [SerializeField]
    private float targetZoomSpeed;
    [SerializeField]
    private LayerMask targetRaycastLayermask;

    private float currentTargetDistance;


    private bool phaseOneComplete = false;

    private GameObject instantiatedTargetLocationMarker;

    private void Start()
    {

    }

    private void Update()
    {
        bool triggered = Input.GetButtonDown(InputNames.Ability_Trigger);
        if (triggered)
        {
            if (!phaseOneComplete)
            {
                TriggerPhaseOne();
            }
            else
            {
                TriggerPhaseTwo();
            }
        }
        else
        {
            float zoomInput = Input.GetAxis(InputNames.Scroll);
            if (phaseOneComplete)
            {
                ZoomTarget(zoomInput);
            }
        }
    }

    private void TriggerPhaseOne()
    {
        cameraController.AttemptTakeControl(
            this,
            OnTakeCameraControlSuccess,
            OnTakeCameraControlFailure);
    }

    private void TriggerPhaseTwo()
    {
        Destroy(instantiatedTargetLocationMarker);
        Vector3 cameraPosition = cameraController.GetCameraTransform().position;
        playerRigidbody.MovePosition(instantiatedTargetLocationMarker.transform.position);
        cameraController.GetCameraTransform().position = cameraPosition;
        cameraController.AttemptReleaseControl(this);
        phaseOneComplete = false;
    }

    private void OnTakeCameraControlSuccess()
    {
        //playerMovement.enabled = false;
        cameraController.AttemptDisableCameraZoom(this);
        cameraController.SetTargetZoom(0.0f, this);

        currentTargetDistance = minTargetDistance;
        Vector3 targetPosition = cameraController.GetCameraTransform().position + (cameraController.GetCameraTransform().forward * minTargetDistance);
        Quaternion targetRotation = cameraController.GetCameraTransform().rotation;
        instantiatedTargetLocationMarker = Instantiate(targetLocationMarkerPrefab, targetPosition, targetRotation);
        instantiatedTargetLocationMarker.transform.SetParent(cameraController.GetCameraTransform());

        phaseOneComplete = true;
    }

    private void OnTakeCameraControlFailure()
    {

    }

    private void ZoomTarget(float zoomInput)
    {
        float deltaZoom = zoomInput * targetZoomSpeed * Time.deltaTime;
        Vector3 direction = Vector3.forward;
        currentTargetDistance = Mathf.Clamp(currentTargetDistance + deltaZoom, minTargetDistance, maxTargetDistance);
        Vector3 targetLocalPosition = direction * currentTargetDistance;

        RaycastHit hit;
        Transform cameraTransform = cameraController.GetCameraTransform();
#if UNITY_EDITOR
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * currentTargetDistance, Color.red);
#endif
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, currentTargetDistance, targetRaycastLayermask))
        {
            instantiatedTargetLocationMarker.transform.position = hit.point;
        }
        else
        {
            instantiatedTargetLocationMarker.transform.localPosition = targetLocalPosition;
        }
    }

    private void GetTargetLocation()
    {

    }
}
