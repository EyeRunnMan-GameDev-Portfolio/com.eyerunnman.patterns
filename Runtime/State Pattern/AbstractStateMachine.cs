using System;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractStateMachine<Context,TriggerEnum,StateEnum> : MonoBehaviour  where StateEnum : Enum where TriggerEnum : Enum
    {
        protected internal IFactory<AbstractTriggerableState<Context, TriggerEnum, StateEnum>, StateEnum> StateFactory { get; set; }
        protected internal AbstractTriggerableState<Context, TriggerEnum, StateEnum> CurrentState { get; set; }
        protected internal Context Ctx { get; set; }
    }

}

