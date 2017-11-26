using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teleportation : PlayerAbility
{
    #region Serialized Variables
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
    #endregion

    #region Private Variables
    private float currentTargetDistance;

    private GameObject instantiatedTargetLocationMarker;
    #endregion

    #region Entry State
    private MonoFSM.State stateEntry;

    private void EntryStateOnCheckTransitions()
    {
        bool triggered = Input.GetButtonDown(InputNames.Ability_Trigger);
        if (triggered)
        {
            if (cameraController.AttemptTakeControl(this))
            {
                Transition(stateChooseTargetLocation);
            }
            else
            {
                //TODO: Implement a failure case if theres a chance something else could be controlling the camera
            }
        }
    }
    #endregion

    #region ChooseTargetLocation State
    private MonoFSM.State stateChooseTargetLocation;

    private void ChooseTargetLocationStateOnEnter()
    {
        //playerMovement.enabled = false;
        cameraController.AttemptDisableCameraZoom(this);
        cameraController.SetTargetZoom(0.0f, this);

        currentTargetDistance = minTargetDistance;
        Vector3 targetPosition = cameraController.GetCameraTransform().position + (cameraController.GetCameraTransform().forward * minTargetDistance);
        Quaternion targetRotation = cameraController.GetCameraTransform().rotation;
        instantiatedTargetLocationMarker = Instantiate(targetLocationMarkerPrefab, targetPosition, targetRotation);
        instantiatedTargetLocationMarker.transform.SetParent(cameraController.GetCameraTransform());
    }

    private void ChooseTargetLocationStateOnUpdate()
    {
        float zoomInput = Input.GetAxis(InputNames.Scroll);
        ZoomTarget(zoomInput);
    }

    private void ChooseTargetLocationStateOnCheckTransitions()
    {
        bool triggered = Input.GetButtonDown(InputNames.Ability_Trigger);
        if (triggered)
        {
            Vector3 cameraPosition = cameraController.GetCameraTransform().position;
            playerRigidbody.MovePosition(instantiatedTargetLocationMarker.transform.position);
            cameraController.GetCameraTransform().position = cameraPosition;

            Transition(stateEntry);
        }
        else
        {
            bool canceled = Input.GetButtonDown(InputNames.Ability_Cancel);

            if (canceled)
            {
                Transition(stateEntry);
            }
        }
    }

    private void ChooseTargetLocationStateOnExit()
    {
        playerRigidbody.MoveRotation(Quaternion.Euler(0.0f, instantiatedTargetLocationMarker.transform.rotation.y, 0.0f));
        cameraController.AttemptReleaseControl(this);
        Destroy(instantiatedTargetLocationMarker);
    }
    #endregion

    private void Start()
    {
        stateEntry = new MonoFSM.State(
            null,
            null,
            null,
            null,
            EntryStateOnCheckTransitions,
            null
        );

        stateChooseTargetLocation = new MonoFSM.State(
            ChooseTargetLocationStateOnEnter,
            ChooseTargetLocationStateOnUpdate,
            null,
            null,
            ChooseTargetLocationStateOnCheckTransitions,
            ChooseTargetLocationStateOnExit
        );

        Transition(stateEntry);
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
}
