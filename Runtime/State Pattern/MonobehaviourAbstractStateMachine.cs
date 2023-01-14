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

        protected abstract StateEnum RootEnum { get; }
        protected internal IFactory<AbstractTriggerableState<Context, StateEnum, TriggerEnum>, StateEnum> StateFactory { get; set; }
        protected internal AbstractTriggerableState<Context, StateEnum, TriggerEnum> CurrentRootState { get; set; }
        protected internal Context Ctx { get; set; }
        protected internal HashSet<TriggerEnum> CurrentFrameTriggers { get; protected set; }
        protected IAbstractStateMachine<Context, StateEnum, TriggerEnum>.StateTreeNode StateTreeNode { get; private set; }

        protected internal List<StateEnum> CurrentStateChain => InterfaceObjectRef.CurrentStateChain;

        protected void ComputeStateTree()
        {
            InterfaceObjectRef.ComputeStateTree();
        }
    }
}

