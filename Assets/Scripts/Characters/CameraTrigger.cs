using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnCameraTriggerEnter(Collider enteredCollider);
public delegate void OnCameraTriggerExit(Collider exitedCollider);

public class CameraTrigger : MonoBehaviour
{

    private OnCameraTriggerEnter onTriggerEnter;
    private OnCameraTriggerExit onTriggerExit;

    public void Subscribe(OnCameraTriggerEnter enterCallback, OnCameraTriggerExit exitCallback)
    {
        onTriggerEnter += enterCallback;
        onTriggerExit += exitCallback;
    }

    private void OnTriggerEnter(Collider enteredCollider)
    {
        onTriggerEnter(enteredCollider);
    }

    private void OnTriggerExit(Collider exitedCollider)
    {
        onTriggerExit(exitedCollider);
    }
}
