using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class MonobehaviourAbstractStateMachine<Context, StateEnum, TriggerEnum> : MonoBehaviour, IAbstractStateMachine<Context, StateEnum, TriggerEnum> where StateEnum : Enum where TriggerEnum : Enum
    {
        StateEnum IAbstractStateMachine<Context, StateEnum, TriggerEnum>.RootEnum => RootEnum;
        IFactory<AbstractTriggerableState<Context, StateEnum, TriggerEnum>, StateEnum> IAbstractStateMachine<Context, StateEnum, TriggerEnum>.StateFactory => StateFactory;
        AbstractTriggerableState<Context, StateEnum, TriggerEnum> IAbstractStateMachine<Context, StateEnum, TriggerEnum>.CurrentRootState { get => CurrentRootState; set => CurrentRootState = value; }
        Context IAbstractStateMachine<Context, StateEnum, TriggerEnum>.Ctx => Ctx;
        HashSet<TriggerEnum> IAbstractStateMachine<Context, StateEnum, TriggerEnum>.CurrentFrameTriggers => CurrentFrameTriggers;

        IAbstractStateMachine<Context, StateEnum, TriggerEnum>.StateTreeNode IAbstractStateMachine<Context, StateEnum, TriggerEnum>.StateTree { get => StateTreeNode; set => StateTreeNode = value; }

        private IAbstractStateMachine<Context, StateEnum, TriggerEnum> InterfaceObjectRef => this as IAbstractStateMachine<Context, StateEnum, TriggerEnum>;

        /// <summary>
        /// Root State Enum , the enum should have a value as root and give that value to this variable
        /// NOTE : this is not an actual state but is used as the root node for computing 
        /// </summary>
        protected abstract StateEnum RootEnum { get; }
        /// <summary>
        /// Set this variable in the awake function 
        /// </summary>
        protected internal IFactory<AbstractTriggerableState<Context, StateEnum, TriggerEnum>, StateEnum> StateFactory { get; set; }
        /// <summary>
        /// Set this variable in the the awake function 
        /// </summary>
        protected internal AbstractTriggerableState<Context, StateEnum, TriggerEnum> CurrentRootState { get; set; }
        /// <summary>
        /// Set the ctx in awake
        /// </summary>
        protected internal Context Ctx { get; set; }
        /// <summary>
        /// Set CurrentFrame Triggers list in the update of the child class itself
        /// </summary>
        protected internal HashSet<TriggerEnum> CurrentFrameTriggers { get; protected set; }
        protected IAbstractStateMachine<Context, StateEnum, TriggerEnum>.StateTreeNode StateTreeNode { get; private set; }

        /// <summary>
        /// to get the list of current active state chain
        /// </summary>
        protected internal List<StateEnum> CurrentStateChain => InterfaceObjectRef.CurrentStateChain;

        /// <summary>
        /// call this function in awake to compute the state machine
        /// </summary>
        protected void ComputeStateTree()
        {
            InterfaceObjectRef.ComputeStateTree();
        }
    }
}

