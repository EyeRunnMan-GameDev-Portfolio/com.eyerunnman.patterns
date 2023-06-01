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

        protected bool IsPaused { get; private set; } = false;
        #endregion

        #region State Properties
        /// <summary>
        /// state id
        /// </summary>
        public StateEnum StateID { get; private set; }
        /// <summary>
        /// to handle triggers or not
        /// </summary>
        protected Status TriggerStatus { get; private set; }

        /// <summary>
        /// set the undefined state
        /// </summary>
        protected abstract StateEnum UndefinedState { get; }

        /// <summary>
        /// get the enum of current child state
        /// </summary>
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

        /// <summary>
        /// Is root state
        /// NOTE : If you want this to be true use root state abstract class
        /// </summary>
        protected internal virtual bool IsRootState => false;

        /// <summary>
        /// Is current state child state
        /// </summary>
        protected internal bool IsChildState => ParentStateRef != null;
        /// <summary>
        /// is current state parent state
        /// </summary>
        protected internal bool IsParentState => ActiveChildStateNode != null;

        private AbstractTriggerableState<Context, StateEnum, TriggerEnum> ParentStateRef { get; set; }
        internal AbstractTriggerableState<Context, StateEnum, TriggerEnum> ActiveChildStateNode { get; private set; }

        #endregion

        #region Transition Properties
        internal HashSet<StateEnum> LoadedChildStates { get; private set; }
        private StateEnum DefaultChildEnum {get;set;}
        private HashSet<StateEnum> InternalStateTransitions { get; set; }
        #endregion

        #region IState

        /// <summary>
        /// call in awake when setting up the first state
        /// </summary>
        public void ExecuteStateEnter()
        {
            OnStateAwake();
            SetCurrentDefaultChildState();
            EnterChildState();
        }

        /// <summary>
        /// To Pause the state and its child states
        /// </summary>
        public void ExecuteStatePause()
        {
            if(!IsPaused)
            {
                OnStatePause();
            }
            PauseChildStates();
            IsPaused = true;
        }

        /// <summary>
        /// To Resume the state and its child states
        /// </summary>
        public void ExecuteStateResume()
        {
            if (IsPaused)
            {
                OnStateResume();
            }
            ResumeChildStates();
            IsPaused = false;
        }

        private void SetCurrentDefaultChildState()
        {
            if (LoadedChildStates.Contains(DefaultChildEnum))
                SetCurrentChildState(StateFactory.Create(DefaultChildEnum));
        }

        /// <summary>
        /// Execute State Update in the update method
        /// </summary>
        public void ExecuteStateUpdate()
        {
            if (!IsPaused)
            {
                OnStateUpdate();
                UpdateChildStates();
            }
        }

        /// <summary>
        /// execute state fixed update in fixed update
        /// </summary>
        public void ExecuteStateFixedUpdate()
        {
            if (!IsPaused)
            {
                OnStateFixedUpdate();
                FixedUpdateChildStates();
            }
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
        /// <summary>
        /// Is called on state Setup
        /// </summary>
        protected virtual void OnStateSetup(){}
        /// <summary>
        /// Is called on State Awake
        /// </summary>
        protected virtual void OnStateAwake() { }
        /// <summary>
        /// is called on State Update
        /// </summary>
        protected virtual void OnStateUpdate(){}
        /// <summary>
        /// is called on State Fixed Update
        /// </summary>
        protected virtual void OnStateFixedUpdate(){}
        /// <summary>
        /// Is called on state fixed update
        /// </summary>
        protected virtual void OnStateExit() { }
        /// <summary>
        /// When Triggers are fired
        /// </summary>
        protected virtual void OnMultipleTriggers(){}

        /// <summary>
        /// When Triggers are fired
        /// </summary>
        protected virtual void OnStatePause() { }

        /// <summary>
        /// When Triggers are fired
        /// </summary>
        protected virtual void OnStateResume() { }
        #endregion

        /// <summary>
        /// call this in the child update after calculating the list of triggers
        /// </summary>
        public void ExecuteMultipleTriggers()
        {
            if (!IsPaused)
            {
                if (TriggerStatus.Enabled)
                {
                    OnMultipleTriggers();
                    ActiveChildStateNode?.ExecuteMultipleTriggers();
                }
            }
            
        }

        #region Transition Execution Methods
        /// <summary>
        /// Invoke this in child for transition
        /// </summary>
        /// <param name="transitionToState">the state to transition to </param>
        protected void InvokeTransition(StateEnum transitionToState)
        {
            if (InternalStateTransitions.Contains(transitionToState))
            {
                SwitchState(StateFactory.Create(transitionToState));
            }
        }
        /// <summary>
        /// Invoke this in child for transition
        /// </summary>
        /// <param name="transitionToState">the state to transition to </param>
        /// <param name="activeChildOfTargetState">the active child that should be in the next state</param>
        protected void InvokeTransition(StateEnum transitionToState, StateEnum activeChildOfTargetState)
        {
            if (InternalStateTransitions.Contains(transitionToState))
            {
                SwitchState(transitionToState, activeChildOfTargetState);
            }
        }

        #endregion

        #region State Initilization Initilization Methods

        /// <summary>
        /// add child to child state in the setup lifecycle
        /// </summary>
        /// <param name="stateEnum"></param>
        protected void LoadChildState(StateEnum stateEnum)
        {
            LoadedChildStates.Add(stateEnum);

            if (LoadedChildStates.Count == 1)
            {
                SetDefaultChildState(stateEnum);
            }
        }
        /// <summary>
        /// add all the allowed states in the setup lifecycle
        /// </summary>
        /// <param name="stateEnum"></param>
        protected void AddTransition(StateEnum stateEnum)
        {
            InternalStateTransitions.Add(stateEnum);
        }

        /// <summary>
        /// set default child state 
        /// </summary>
        /// <param name="stateEnum"></param>
        protected void SetDefaultChildState(StateEnum stateEnum)
        {
            DefaultChildEnum = stateEnum;
        }

        #endregion

        #region Triggers Check Methods
        /// <summary>
        /// check if triggers are present in current frame check in the trigger lifecycle
        /// </summary>
        /// <param name="triggersToCompare">list of triggers to compare to</param>
        /// <returns></returns>
        protected bool AreTriggersPresentCurrentFrame(List<TriggerEnum> triggersToCompare)
        {
            if (triggersToCompare.All(trigger => IsTriggerPresentCurrentFrame(trigger)))
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// check if single triggers is present in current frame check in the trigger lifecycle
        /// </summary>
        /// <param name="triggerToCompare">trigger to comare to</param>
        /// <returns></returns>
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
        private void PauseChildStates()
        {
            if (IsParentState)
            {
                ActiveChildStateNode?.ExecuteStatePause();
            }
        }

        private void ResumeChildStates()
        {
            if (IsParentState)
            {
                ActiveChildStateNode?.ExecuteStateResume();
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

