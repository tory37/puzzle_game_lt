using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// How to use:
///     - Create class that inherits from MonoFSM.
///     - For each needed state, declare variables of type MonoFSM.State.
///         - It is recommended to initialize the variables as the first thing in void Start() to allow access to class methods
///     - In void Start(), call Transition(MonoFSM targetState), where targetState is the initial state of the FSM. 
/// </summary>
public class MonoFSM : MonoBehaviour
{
    public class State
    {
        public string GetStateName()
        {
            return this.GetType().Name;
        }

        public delegate void Phase();
        private MonoFSM.State.Phase onEnter;
        private MonoFSM.State.Phase onUpdate;
        private MonoFSM.State.Phase onFixedUpdate;
        private MonoFSM.State.Phase onLateUpdate;
        private MonoFSM.State.Phase onCheckTransitions;
        private MonoFSM.State.Phase onExit;

        public State(MonoFSM.State.Phase onEnter, MonoFSM.State.Phase onUpdate, MonoFSM.State.Phase onFixedUpdate, MonoFSM.State.Phase onLateUpdate, MonoFSM.State.Phase onCheckTransitions, MonoFSM.State.Phase onExit)
        {
            if (onEnter != null)
                this.onEnter += onEnter;
            if (onUpdate != null)
                this.onUpdate += onUpdate;
            if (onFixedUpdate != null)
                this.onFixedUpdate += onFixedUpdate;
            if (onLateUpdate != null)
                this.onLateUpdate += onLateUpdate;
            if (onCheckTransitions != null)
                this.onCheckTransitions += onCheckTransitions;
            if (onExit != null)
                this.onExit += onExit;
        }

        public void OnEnter()
        {
            if (onEnter != null)
                onEnter();
        }

        public void OnUpdate()
        {
            if (onUpdate != null)
                onUpdate();
        }

        public void OnFixedUpdate()
        {
            if (onFixedUpdate != null)
                onFixedUpdate();
        }

        public void OnLateUpdate()
        {
            if (onLateUpdate != null)
                onLateUpdate();
        }

        public void OnChechTransitions()
        {
            if (onCheckTransitions != null)
                onCheckTransitions();
        }

        public void OnExit()
        {
            if (onExit != null)
                onExit();
        }
    }

    #region Variables
    MonoFSM.State currentState;

    #endregion

    #region Mono Methods
    private void Update()
    {
        if (currentState != null)
            currentState.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (currentState != null)
            currentState.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        if (currentState != null)
        {
            currentState.OnLateUpdate();
            currentState.OnChechTransitions();
        }
    }
    #endregion

    #region FSM Methods
    protected void Transition(MonoFSM.State targetState)
    {
        if (targetState != null)
        {
            if (currentState != null)
                currentState.OnExit();

            currentState = targetState;
            currentState.OnEnter();
        }
        else
        {
            Debug.LogError("Attempt to transition failed. Target state cannot be null. FSM: '" + this.name + "'.");
        }
    }
    #endregion

}
