using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractState<T,U> : IState<T>,ITriggerable<U> where U: Enum
    {

        private AbstractStateMachine<T, U> StateContext { get; set; }
        protected T Context { get; private set; }

        protected virtual bool IsRootState => false;

        private bool IsChildState => CurrentParentState != null;
        private bool IsParentState => CurrentChildState != null;

        private AbstractState<T,U> CurrentParentState { get; set; }
        private AbstractState<T, U> CurrentChildState { get; set; }
        private Dictionary<Type, AbstractState<T, U>> LoadedChildStates { get;set; }
        private Dictionary<U, AbstractState<T, U>> Transitions { get;set; }

        public AbstractState(AbstractStateMachine<T,U> stateContext){

            StateContext = stateContext;
            Context = stateContext.Context;
            LoadedChildStates = new();
            Transitions = new();
        }

        public virtual void OnEnterState()
        {
            EnterChildState();
        }

        public virtual void OnUpdateState()
        {
            UpdateChildStates();
        }

        private void OnExitState()
        {

        }

        public void OnTrigger(U triggerEnum)
        {
            if (Transitions.TryGetValue(triggerEnum, out AbstractState<T,U> toState))
                SwitchState(toState);
            else
                CurrentChildState?.OnTrigger(triggerEnum);
        }

        protected void LoadChildState(AbstractState<T, U> childState)
        {
            if (LoadedChildStates.Count == 0)
            {
                SetCurrentChildState(childState);
            }

            LoadedChildStates[childState.GetType()] = childState;
        }

        protected void AddTransition(AbstractState<T, U> transitionToState,U transitionTriggerEnum)
        {
            Transitions[transitionTriggerEnum] = transitionToState;
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

        private void SwitchState(AbstractState<T, U> newState)
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

        private void SetCurrentChildState(AbstractState<T, U> childState)
        {
            CurrentChildState = childState;
            childState.SetCurrentParentState(this);
        }
        private void SetCurrentParentState(AbstractState<T, U> newSuperState)
        {
            CurrentParentState = newSuperState;
        }


    }
}

