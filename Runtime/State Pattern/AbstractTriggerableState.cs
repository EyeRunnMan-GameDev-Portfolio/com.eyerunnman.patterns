using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractTriggerableState<Context, TriggerEnums, StateEnum> : IState<Context> where TriggerEnums : Enum where StateEnum : Enum
    {

        
        #region Context Getters
        private AbstractStateMachine<Context, TriggerEnums, StateEnum> StateContext { get; set; }

        protected AbstractTriggerableState<Context, TriggerEnums, StateEnum> CurrentContextState => StateContext.CurrentRootState;
        protected IFactory<AbstractTriggerableState<Context, TriggerEnums, StateEnum>, StateEnum> StateFactory => StateContext.StateFactory;
        protected Context Ctx => StateContext.Ctx;
        protected HashSet<TriggerEnums> CurrentFrameTriggers => StateContext.CurrentFrameTriggers;
        #endregion

        #region State Properties
        internal StateEnum StateID { get; private set; }
        protected Status TriggerStatus { get; private set; }

        #endregion

        #region State Properties
        protected internal virtual bool IsRootState => false;

        internal bool IsChildState => ParentStateRef != null;
        internal bool IsParentState => ChildStateNode != null;

        private AbstractTriggerableState<Context, TriggerEnums, StateEnum> ParentStateRef { get; set; }
        internal AbstractTriggerableState<Context, TriggerEnums, StateEnum> ChildStateNode { get; private set; }

        #endregion

        #region Transition Properties
        internal HashSet<StateEnum> ChildNodeEnums { get; private  set; }

        private HashSet<StateEnum> InternalStateTransitions { get; set; }
        #endregion

        #region IState
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
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="stateID"></param>
        public AbstractTriggerableState(AbstractStateMachine<Context, TriggerEnums, StateEnum> context, StateEnum stateID)
        {

            StateContext = context;

            StateID = stateID;

            ChildNodeEnums = new();
            InternalStateTransitions = new();

            TriggerStatus = new();
        }

        #region State LifeCycle
        protected abstract void OnStateEnter();
        protected abstract void OnStateUpdate();
        protected abstract void OnStateExit();
        protected abstract void OnMultipleTriggers();
        #endregion

        public void ExecuteMultipleTriggers()
        {
            if (TriggerStatus.Enabled)
            {
                OnMultipleTriggers();
                ChildStateNode?.OnMultipleTriggers();
            }
        }

        #region Transition Execution Methods

        protected void InvokeTransition(StateEnum transitionToState,bool sendToParent=false)
        {
            if (InternalStateTransitions.Contains(transitionToState))
            {
                SwitchState(StateFactory.Create(transitionToState));
            }
            else
            {
                if (IsChildState)
                {
                    ParentStateRef.InvokeTransition(transitionToState, sendToParent);
                }
            }
        }

        #endregion

        #region State Initilization Initilization Methods

        protected void LoadChildState(StateEnum stateEnum)
        {
            ChildNodeEnums.Add(stateEnum);

            if (ChildNodeEnums.Count == 1)
            {
                SetCurrentChildState(StateFactory.Create(stateEnum));
            }
        }
        protected void AddTransition(StateEnum stateEnum)
        {
            InternalStateTransitions.Add(stateEnum);
        }

        #endregion

        #region Triggers Check Methods
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
        #endregion

        #region Child State LifeCycle Execution Methods
        private void EnterChildState()
        {
            if (IsParentState)
            {
                ChildStateNode.ExecuteStateEnter();
            }
        }
        private void UpdateChildStates()
        {
            if (IsParentState)
            {
                ChildStateNode?.ExecuteStateUpdate();
            }
        }
        #endregion

        #region State Switch Methods
        private void SwitchState(AbstractTriggerableState<Context, TriggerEnums, StateEnum> newState)
        {
            if (newState == null)
            {
                return;
            }

            if (IsRootState && newState.IsRootState || IsChildState && newState.IsChildState)
            {
                ExecuteStateExit();
                newState.ExecuteStateEnter();

                if (IsRootState)
                {
                    StateContext.CurrentRootState = newState;
                }
                else if (IsChildState)
                {
                    ParentStateRef.SetCurrentChildState(newState);
                }
            }

        }

        private void SetCurrentChildState(AbstractTriggerableState<Context, TriggerEnums, StateEnum> childState)
        {
            if (ChildNodeEnums.Contains(childState.StateID))
            {
                ChildStateNode = childState;
                childState.SetCurrentParentState(this);
            }
            else
            {
                childState.ExecuteStateExit();
            }
        }
        private void SetCurrentParentState(AbstractTriggerableState<Context, TriggerEnums, StateEnum> newSuperState)
        {
            ParentStateRef = newSuperState;
        }
        #endregion

    }
}

