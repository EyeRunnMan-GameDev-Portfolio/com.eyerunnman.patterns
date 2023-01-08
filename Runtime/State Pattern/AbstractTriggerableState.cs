using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractTriggerableState<Context,TriggerEnums,StateEnum> : IState<Context>,ITriggerable<TriggerEnums> where TriggerEnums: Enum where StateEnum: Enum
    {

        private AbstractStateMachine<Context, TriggerEnums, StateEnum> StateContext { get; set; }

        protected AbstractTriggerableState<Context, TriggerEnums, StateEnum> CurrentContextState => StateContext.CurrentState;
        protected IFactory<AbstractTriggerableState<Context, TriggerEnums, StateEnum>, StateEnum> StateFactory => StateContext.StateFactory;
        protected Context Ctx => StateContext.Ctx;

        protected StateEnum StateID { get; set; }
        protected virtual bool IsRootState => false;

        private bool IsChildState => CurrentParentState != null;
        private bool IsParentState => CurrentChildState != null;

        private AbstractTriggerableState<Context,TriggerEnums, StateEnum> CurrentParentState { get; set; }
        private AbstractTriggerableState<Context, TriggerEnums, StateEnum> CurrentChildState { get; set; }
        private HashSet<StateEnum> LoadedChildStates { get;set; }
        private Dictionary<TriggerEnums, StateEnum> Transitions { get; set; }

        public AbstractTriggerableState(AbstractStateMachine<Context, TriggerEnums, StateEnum> context,StateEnum stateID){

            StateContext = context;

            StateID = stateID;

            LoadedChildStates = new();
            Transitions = new();
        }

        public virtual void OnEnterState()
        {
            Debug.Log((IsRootState ? ("Root : ") : ("-> ")) + StateID.ToString() + " Enter ");
            EnterChildState();
        }

        public virtual void OnUpdateState()
        {
            Debug.Log((IsRootState ? ("Root : ") : ("-> ")) + StateID.ToString() + " Update ");
            UpdateChildStates();
        }

        public virtual void OnExitState()
        {

        }

        public void OnTrigger(TriggerEnums triggerEnum)
        {
            if (Transitions.TryGetValue(triggerEnum, out StateEnum toState))
                SwitchState(StateFactory.Create(toState));
                
            CurrentChildState?.OnTrigger(triggerEnum);
        }

        protected void LoadChildState(StateEnum stateEnum)
        {
            LoadedChildStates.Add(stateEnum);

            if (LoadedChildStates.Count == 1)
            {
                SetCurrentChildState(StateFactory.Create(stateEnum));
            }
        }

        protected void AddTransition(StateEnum stateEnum,TriggerEnums transitionTriggerEnum)
        {
            Transitions[transitionTriggerEnum] = stateEnum;
        }

        private void EnterChildState()
        {
            if (IsParentState)
            {
                CurrentChildState.OnEnterState();
            }
        }
        private void UpdateChildStates()
        {
            if (IsParentState)
            {
                CurrentChildState.OnUpdateState();
            }
        }

        private void SwitchState(AbstractTriggerableState<Context, TriggerEnums, StateEnum> newState)
        {
            if(newState == null)
            {
                return;
            }

            if(IsRootState && newState.IsRootState || IsChildState && newState.IsChildState)
            {
                OnExitState();
                newState.OnEnterState();

                if (IsRootState)
                {
                    StateContext.CurrentState = newState;
                }
                else if(IsChildState)
                {
                    CurrentParentState.SetCurrentChildState(newState);
                }
            }

        }

        private void SetCurrentChildState(AbstractTriggerableState<Context, TriggerEnums, StateEnum> childState)
        {
            if (LoadedChildStates.Contains(childState.StateID))
            {
                CurrentChildState = childState;
                childState.SetCurrentParentState(this);
            }
        }
        private void SetCurrentParentState(AbstractTriggerableState<Context, TriggerEnums, StateEnum> newSuperState)
        {
            CurrentParentState = newSuperState;
        }
    }
}

