using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractStateMachine<Context, TriggerEnum, StateEnum> : MonoBehaviour where StateEnum : Enum where TriggerEnum : Enum
    {
        protected internal IFactory<AbstractTriggerableState<Context, TriggerEnum, StateEnum>, StateEnum> StateFactory { get; set; }
        protected internal AbstractTriggerableState<Context, TriggerEnum, StateEnum> CurrentRootState { get; set; }


        protected internal List<AbstractTriggerableState<Context, TriggerEnum, StateEnum>> CurrentStateChain { get {

                List < AbstractTriggerableState < Context, TriggerEnum, StateEnum >> chain = new();

                AbstractTriggerableState < Context, TriggerEnum, StateEnum > stateRef = CurrentRootState;

                if (stateRef is not null)
                {
                    chain.Add(stateRef);
                }

                while (stateRef?.CurrentChildState is not null)
                {
                    chain.Add(stateRef?.CurrentChildState);
                    stateRef = stateRef?.CurrentChildState;
                }

                return chain;
            } }

        protected internal Context Ctx { get; set; }
        protected internal HashSet<TriggerEnum> CurrentFrameTriggers { get; set; }
    }

}

