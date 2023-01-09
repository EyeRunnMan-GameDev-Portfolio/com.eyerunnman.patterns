using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractTriggerableState<Context, TriggerEnums, StateEnum> : IState<Context> where TriggerEnums : Enum where StateEnum : Enum
    {

        private AbstractStateMachine<Context, TriggerEnums, StateEnum> StateContext { get; set; }

        protected AbstractTriggerableState<Context, TriggerEnums, StateEnum> CurrentContextState => StateContext.CurrentRootState;
        protected IFactory<AbstractTriggerableState<Context, TriggerEnums, StateEnum>, StateEnum> StateFactory => StateContext.StateFactory;
        protected Context Ctx => StateContext.Ctx;

        protected HashSet<TriggerEnums> CurrentFrameTriggers => StateContext.CurrentFrameTriggers;
        protected virtual bool IsRootState => false;

        public StateEnum StateID { get; private set; }

        protected Status TriggerStatus { get; private set; }

        private bool IsChildState => CurrentParentState != null;
        private bool IsParentState => CurrentChildState != null;

        private AbstractTriggerableState<Context,TriggerEnums, StateEnum> CurrentParentState { get; set; }
        internal AbstractTriggerableState<Context, TriggerEnums, StateEnum> CurrentChildState { get; set; }
        private HashSet<StateEnum> LoadedChildStates { get;set; }

        private HashSet<StateEnum> Transitions { get; set; }


        public AbstractTriggerableState(AbstractStateMachine<Context, TriggerEnums, StateEnum> context,StateEnum stateID){

            StateContext = context;

            StateID = stateID;

            LoadedChildStates = new();
            Transitions = new();

            TriggerStatus = new();
        }

        public virtual void ExecuteStateEnter()
        {
            Debug.Log((IsRootState ? ("Root : ") : ("-> ")) + StateID.ToString() + " Enter ");
            OnStateEnter();
            EnterChildState();
        }

        public virtual void ExecuteStateUpdate()
        {
            Debug.Log((IsRootState ? ("Root : ") : ("-> ")) + StateID.ToString() + " Update ");
            OnStateUpdate();
            UpdateChildStates();
        }

        public void ExecuteStateExit()
        {
            OnStateExit();
        }

        protected abstract void OnStateEnter();

        protected abstract void OnStateUpdate();

        protected abstract void OnStateExit();

        protected abstract void OnMultipleTriggers();

        public void ExecuteMultipleTriggers()
        {
            if (TriggerStatus.Enabled)
            {
                OnMultipleTriggers();
                CurrentChildState?.OnMultipleTriggers();
            }
        }


        protected void InvokeTransition(StateEnum transitionToState)
        {
            if(Transitions.Contains(transitionToState))
            {
                SwitchState(StateFactory.Create(transitionToState));
            }
        }

        protected void LoadChildState(StateEnum stateEnum)
        {
            LoadedChildStates.Add(stateEnum);

            if (LoadedChildStates.Count == 1)
            {
                SetCurrentChildState(StateFactory.Create(stateEnum));
            }
        }

        protected void AddTransition(StateEnum stateEnum)
        {
            Transitions.Add(stateEnum);
        }
        protected bool AreTriggersPresentCurrentFrame(List<TriggerEnums> triggersToCompare)
        {
            if (triggersToCompare.All(trigger => IsTriggerPresentCurrentFrame(trigger)))
            {
                return true;
            }
            else
                return false;
        }
        protected bool IsTriggerPresentCurrentFrame(TriggerEnums triggerToCompare)
        {
            return CurrentFrameTriggers.Contains(triggerToCompare);
        }

        private void EnterChildState()
        {
            if (IsParentState)
            {
                CurrentChildState.ExecuteStateEnter();
            }
        }
        private void UpdateChildStates()
        {
            if (IsParentState)
            {
                CurrentChildState?.ExecuteStateUpdate();
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
                ExecuteStateExit();
                newState.ExecuteStateEnter();

                if (IsRootState)
                {
                    StateContext.CurrentRootState = newState;
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
            else
            {
                childState.ExecuteStateExit();
            }
        }
        private void SetCurrentParentState(AbstractTriggerableState<Context, TriggerEnums, StateEnum> newSuperState)
        {
            CurrentParentState = newSuperState;
        }

       
    }
}

