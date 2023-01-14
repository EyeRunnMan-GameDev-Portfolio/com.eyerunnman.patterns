using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractTriggerableState<Context, StateEnum, TriggerEnum> : IState<Context> where TriggerEnum : Enum where StateEnum : Enum
    {


        #region Context Getters
        private IAbstractStateMachine<Context, StateEnum, TriggerEnum> StateContext { get; set; }

        protected AbstractTriggerableState<Context, StateEnum, TriggerEnum> CurrentContextState => StateContext.CurrentRootState;
        protected IFactory<AbstractTriggerableState<Context, StateEnum, TriggerEnum>, StateEnum> StateFactory => StateContext.StateFactory;
        protected Context Ctx => StateContext.Ctx;
        protected HashSet<TriggerEnum> CurrentFrameTriggers => StateContext.CurrentFrameTriggers;
        #endregion

        #region State Properties
        public StateEnum StateID { get; private set; }
        protected Status TriggerStatus { get; private set; }

        protected abstract StateEnum UndefinedState { get; }


        protected StateEnum CurrentChildState
        {
            get
            {
                if (IsParentState)
                {
                    return ActiveChildStateNode.StateID;
                }

                return UndefinedState;
            }
        }

        protected internal virtual bool IsRootState => false;

        internal bool IsChildState => ParentStateRef != null;
        internal bool IsParentState => ActiveChildStateNode != null;

        private AbstractTriggerableState<Context, StateEnum, TriggerEnum> ParentStateRef { get; set; }
        internal AbstractTriggerableState<Context, StateEnum, TriggerEnum> ActiveChildStateNode { get; private set; }

        #endregion

        #region Transition Properties
        internal HashSet<StateEnum> LoadedChildStates { get; private set; }
        private StateEnum DefaultChildEnum {get;set;}
        private HashSet<StateEnum> InternalStateTransitions { get; set; }
        #endregion

        #region IState

        public virtual void ExecuteStateEnter()
        {
            OnStateAwake();
            SetCurrentDefaultChildState();
            EnterChildState();
        }

        private void SetCurrentDefaultChildState()
        {
            if (LoadedChildStates.Contains(DefaultChildEnum))
                SetCurrentChildState(StateFactory.Create(DefaultChildEnum));
        }

        public virtual void ExecuteStateUpdate()
        {
            OnStateUpdate();
            UpdateChildStates();
        }

        public virtual void ExecuteStateFixedUpdate()
        {
            OnStateFixedUpdate();
            FixedUpdateChildStates();
        }

        public void ExecuteStateExit()
        {
            OnStateExit();
        }
        #endregion

        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stateID"></param>
        public AbstractTriggerableState(IAbstractStateMachine<Context, StateEnum, TriggerEnum> context, StateEnum stateID)
        {
            StateContext = context;

            StateID = stateID;

            LoadedChildStates = new();
            InternalStateTransitions = new();
            TriggerStatus = new();

            OnStateSetup();
        }


        #region State LifeCycle

        protected virtual void OnStateSetup(){}
        protected virtual void OnStateAwake() { }
        protected virtual void OnStateUpdate(){}
        protected virtual void OnStateFixedUpdate(){}
        protected virtual void OnStateExit() { }
        protected virtual void OnMultipleTriggers(){}
        #endregion

        public void ExecuteMultipleTriggers()
        {
            if (TriggerStatus.Enabled)
            {
                OnMultipleTriggers();
                ActiveChildStateNode?.ExecuteMultipleTriggers();
            }
        }

        #region Transition Execution Methods

        protected void InvokeTransition(StateEnum transitionToState)
        {
            if (InternalStateTransitions.Contains(transitionToState))
            {
                SwitchState(StateFactory.Create(transitionToState));
            }
        }
        protected void InvokeTransition(StateEnum transitionToState, StateEnum activeChildOfTargetState)
        {
            if (InternalStateTransitions.Contains(transitionToState))
            {
                SwitchState(transitionToState, activeChildOfTargetState);
            }
        }

        #endregion

        #region State Initilization Initilization Methods

        protected void LoadChildState(StateEnum stateEnum)
        {
            LoadedChildStates.Add(stateEnum);

            if (LoadedChildStates.Count == 1)
            {
                SetDefaultChildState(stateEnum);
            }
        }

        protected void AddTransition(StateEnum stateEnum)
        {
            InternalStateTransitions.Add(stateEnum);
        }

        protected void SetDefaultChildState(StateEnum stateEnum)
        {
            DefaultChildEnum = stateEnum;
        }

        #endregion

        #region Triggers Check Methods
        protected bool AreTriggersPresentCurrentFrame(List<TriggerEnum> triggersToCompare)
        {
            if (triggersToCompare.All(trigger => IsTriggerPresentCurrentFrame(trigger)))
            {
                return true;
            }
            else
                return false;
        }
        protected bool IsTriggerPresentCurrentFrame(TriggerEnum triggerToCompare)
        {
            return CurrentFrameTriggers.Contains(triggerToCompare);
        }
        #endregion

        #region Child State LifeCycle Execution Methods
        private void EnterChildState()
        {
            if (IsParentState)
            {
                ActiveChildStateNode.ExecuteStateEnter();
            }
        }
        private void UpdateChildStates()
        {
            if (IsParentState)
            {
                ActiveChildStateNode?.ExecuteStateUpdate();
            }
        }

        private void FixedUpdateChildStates()
        {
            if (IsParentState)
            {
                ActiveChildStateNode?.ExecuteStateFixedUpdate();
            }
        }
        #endregion

        #region State Switch Methods
        private void SwitchState(AbstractTriggerableState<Context, StateEnum, TriggerEnum> newState)
        {
            if (!IsRootState)
                if (ParentStateRef.LoadedChildStates.Contains(newState.StateID) is false)
                    return;

            if (newState == null)
            {
                return;
            }

            if (IsRootState && newState.IsRootState || !IsRootState && !newState.IsRootState)
            {
                ExecuteStateExit();
                newState.ExecuteStateEnter();

                if (IsRootState)
                {
                    StateContext.CurrentRootState = newState;

                    if (IsParentState)
                    {
                        newState.SetCurrentChildState(ActiveChildStateNode);
                    }
                }
                else if (IsChildState)
                {
                    ParentStateRef.SetCurrentChildState(newState);
                }
            }
        }

        private void SwitchState(StateEnum switchToState, StateEnum activeChildOfTargetState)
        {
            AbstractTriggerableState < Context,StateEnum ,TriggerEnum > newState = StateFactory.Create(switchToState);

            newState.SetDefaultChildState(activeChildOfTargetState);

            SwitchState(newState);
        }



        private void SetCurrentChildState(AbstractTriggerableState<Context, StateEnum, TriggerEnum> childState)
        {
            if (LoadedChildStates.Contains(childState.StateID))
            {
                ActiveChildStateNode = childState;
                childState.SetCurrentParentState(this);
            }
            else
            {
                childState.ExecuteStateExit();
            }
        }

        private void SetCurrentParentState(AbstractTriggerableState<Context, StateEnum, TriggerEnum> newSuperState)
        {
            ParentStateRef = newSuperState;
        }
        #endregion

    }
}

