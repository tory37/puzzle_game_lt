using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Telekinesis : PlayerAbility
{
    #region Serialized Variables
    [Header("Components")]
    [SerializeField]
    private ThirdPersonCameraController cameraController;
    [Header("Variables")]
    [SerializeField]
    private bool drawDebugLine = true;
    [SerializeField]
    private float grabCheckRadius;
    [SerializeField]
    private float objectStillGrabbedRadius;
    [SerializeField]
    private float farthestObjectDistance;
    [SerializeField]
    private float closestObjectDistance;
    [SerializeField]
    private float objectZoomSpeed;
    [SerializeField]
    private float objectMoveTowardSpeed;
    [SerializeField]
    private LayerMask layermask;
    #endregion

    #region Private Variables
    private Transform inSightsTransform;
    private Transform grabbedTransform;
    private Transform lastParentTransform;
    private float currentObjectDistance;
    #endregion

    #region Entry State
    private MonoFSM.State stateEntry;

    private void EntryStateOnEnter()
    {
#if UNITY_EDITOR
        Debug.Log(this.GetType().Name + ": Entry State Entered.");
#endif
        inSightsTransform = null;
        grabbedTransform = null;
        lastParentTransform = null;
    }

    private void EntryStateOnUpdate()
    {
        RaycastHit hit;
        Transform cameraTransform = cameraController.GetCameraTransform();
#if UNITY_EDITOR
        if (drawDebugLine)
        {
            Vector3 rayDirection = cameraTransform.forward * (farthestObjectDistance + cameraTransform.localPosition.magnitude);
            Debug.DrawRay(cameraTransform.position + (cameraTransform.up * grabCheckRadius), rayDirection, Color.blue);
            Debug.DrawRay(cameraTransform.position + (-cameraTransform.up * grabCheckRadius), rayDirection, Color.blue);
            Debug.DrawRay(cameraTransform.position + (cameraTransform.right * grabCheckRadius), rayDirection, Color.blue);
            Debug.DrawRay(cameraTransform.position + (-cameraTransform.right * grabCheckRadius), rayDirection, Color.blue);
        }
#endif
        if (Physics.SphereCast(
            cameraTransform.position,
            grabCheckRadius,
            cameraTransform.forward,
            out hit,
            cameraTransform.localPosition.magnitude + farthestObjectDistance,
            layermask)
           )
        {
            if (hit.transform != inSightsTransform)
            {
                MultiTagSystem tags = hit.collider.GetComponent<MultiTagSystem>();
                if (tags && tags.HasTag(MultiTag.AffectedByTelekenisis))
                {
                    inSightsTransform = hit.transform;
                    Debug.Log("Target in sights.");
                }
            }
        }
        else
        {
            inSightsTransform = null;
        }
    }

    private void EntryStateOnCheckTransitions()
    {
        bool triggered = Input.GetButtonDown(InputNames.Ability_Trigger);
        if (triggered && inSightsTransform != null)
        {
            grabbedTransform = inSightsTransform;

            Transition(stateObjectGrabbed);
        }
    }
    #endregion

    #region ObjectGrabbed State
    private MonoFSM.State stateObjectGrabbed;

    private void ObjectGrabbedStateOnEnter()
    {
#if UNITY_EDITOR
        Debug.Log(this.GetType().Name + ": Object Grabbed State Entered.");
#endif
        grabbedTransform.GetComponent<Rigidbody>().useGravity = false;
        Transform cameraTransform = cameraController.GetCameraTransform();
        currentObjectDistance = Vector3.Distance(grabbedTransform.position, cameraTransform.position) - cameraTransform.localPosition.magnitude;
    }

    private void ObjectGrabbedStateOnUpdate()
    {
#if UNITY_EDITOR
        if (drawDebugLine)
        {
            Transform cameraTransform = cameraController.GetCameraTransform();
            Vector3 grabbedPositionLocalToCamera = cameraTransform.InverseTransformPoint(grabbedTransform.position);
            float rayDistance = grabbedPositionLocalToCamera.magnitude + 1.0f;
            Vector3 rayDirection = cameraTransform.forward * rayDistance;
            Debug.DrawRay(cameraTransform.position + (cameraTransform.up * objectStillGrabbedRadius), rayDirection, Color.blue);
            Debug.DrawRay(cameraTransform.position + (-cameraTransform.up * objectStillGrabbedRadius), rayDirection, Color.blue);
            Debug.DrawRay(cameraTransform.position + (cameraTransform.right * objectStillGrabbedRadius), rayDirection, Color.blue);
            Debug.DrawRay(cameraTransform.position + (-cameraTransform.right * objectStillGrabbedRadius), rayDirection, Color.blue);
        }
#endif   
    }

    private void ObjectGrabbedStateOnLateUpdate()
    {
        float zoomInput = Input.GetAxis(InputNames.Scroll);
        float deltaZoom = zoomInput * objectZoomSpeed * Time.deltaTime;
        Transform cameraTransform = cameraController.GetCameraTransform();
        Rigidbody grabbedRigidBody = grabbedTransform.GetComponent<Rigidbody>();

        if (deltaZoom > 0)
        {
            currentObjectDistance = Mathf.MoveTowards(currentObjectDistance, farthestObjectDistance, deltaZoom);
        }
        else if (deltaZoom < 0)
        {
            currentObjectDistance = Mathf.MoveTowards(currentObjectDistance, closestObjectDistance, -deltaZoom);
        }

        float cameraDistancePlusObjectDistance = cameraTransform.localPosition.magnitude + currentObjectDistance;
        Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * cameraDistancePlusObjectDistance);

        //grabbedRigidBody.MovePosition(Vector3.MoveTowards(grabbedRigidBody.position, targetPosition, 100.0f));
        grabbedRigidBody.MovePosition(Vector3.MoveTowards(grabbedRigidBody.position, targetPosition, objectMoveTowardSpeed * Time.deltaTime));
    }

    private void ObjectGrabbedStateOnCheckTransitions()
    {
        bool triggered = Input.GetButtonDown(InputNames.Ability_Cancel);
        if (triggered)
        {
            Transition(stateEntry);
        }
        else
        {
            RaycastHit[] hits;
            Transform cameraTransform = cameraController.GetCameraTransform();
            hits = Physics.SphereCastAll(
                cameraTransform.position,
                objectStillGrabbedRadius,
                cameraTransform.forward,
                cameraTransform.localPosition.magnitude + currentObjectDistance + 1f,
                layermask
            );
            List<RaycastHit> hitList = hits.OfType<RaycastHit>().ToList();
            bool objectStillInSight = hitList.Exists(hit => hit.transform == grabbedTransform);
            if (!objectStillInSight)
            {
                Transition(stateEntry);
            }
        }
    }

    private void ObjectGrabbedStateOnExit()
    {
        grabbedTransform.GetComponent<Rigidbody>().useGravity = true;
        ResetTransforms();
    }
    #endregion

    #region Mono Methods
    private void Start()
    {
        stateEntry = new MonoFSM.State(
            EntryStateOnEnter,
            EntryStateOnUpdate,
            null,
            null,
            EntryStateOnCheckTransitions,
            null
        );

        stateObjectGrabbed = new MonoFSM.State(
            ObjectGrabbedStateOnEnter,
            ObjectGrabbedStateOnUpdate,
            null,
            ObjectGrabbedStateOnLateUpdate,
            ObjectGrabbedStateOnCheckTransitions,
            ObjectGrabbedStateOnExit
        );

        // This is just in case something happens and the distance isnt set when entering the state the first time.  
        currentObjectDistance = closestObjectDistance;

        Transition(stateEntry);
    }
    #endregion

    #region Class Methods
    private void ResetTransforms()
    {
        inSightsTransform = null;
        grabbedTransform = null;
    }
    #endregion
}
