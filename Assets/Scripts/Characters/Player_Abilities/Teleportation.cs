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

    [Header("Variables")]
    [SerializeField]
    private float minTargetDistance;
    [SerializeField]
    private float maxTargetDistance;
    [SerializeField]
    private float targetZoomSpeed;


    private bool phaseOneComplete = false;

    private float currentTargetDistance;
    private GameObject instantiatedTargetLocationMarker;

    private void Start()
    {
        currentTargetDistance = minTargetDistance;
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
        }
        else
        {
            float zoomInput = Input.GetAxis(InputNames.Scroll);

        }
    }

    private void TriggerPhaseOne()
    {
        cameraController.AttemptTakeControl(
            this,
            OnTakeCameraControlSuccess,
            OnTakeCameraControlFailure);
    }

    private void OnTakeCameraControlSuccess()
    {
        playerMovement.enabled = false;
        Debug.Log(cameraController.AttemptDisableCameraZoom(this));
        cameraController.SetTargetZoom(0.0f, this);

        Vector3 targetPosition = cameraController.GetCameraTransform().position + (cameraController.GetCameraTransform().forward * minTargetDistance);
        Quaternion targetRotation = cameraController.GetCameraTransform().rotation;
        instantiatedTargetLocationMarker = Instantiate(targetLocationMarkerPrefab, targetPosition, targetRotation);
        instantiatedTargetLocationMarker.transform.SetParent(cameraController.GetCameraTransform());
    }

    private void OnTakeCameraControlFailure()
    {

    }

    private void ZoomTarget(float zoomInput)
    {
        float deltaZoom = zoomInput * targetZoomSpeed * Time.deltaTime;
        Vector3 direction = instantiatedTargetLocationMarker.transform.forward;
        currentTargetDistance = Mathf.Clamp(currentTargetDistance + deltaZoom, minTargetDistance, maxTargetDistance);
        Vector3 targetLocalPosition = instantiatedTargetLocationMarker.transform.forward * currentTargetDistance;
        instantiatedTargetLocationMarker.transform.localPosition = targetLocalPosition;
    }

    private void GetTargetLocation()
    {

    }
}
