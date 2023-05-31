using com.eyerunnman.patterns;
using System;

namespace com.eyerunnman.patterns
{
    public abstract class RootAbstractTriggerableState<Context, StateEnum, TriggerEnum> : AbstractTriggerableState<Context, StateEnum, TriggerEnum> where TriggerEnum : Enum where StateEnum : Enum
    {
        public RootAbstractTriggerableState(IAbstractStateMachine<Context, StateEnum, TriggerEnum> context, StateEnum stateID) :base(context,stateID)
        {

        }
        /// <summary>
        /// is root state for current state is true for root abstract state
        /// </summary>
        protected internal override bool IsRootState => true;
    }
}


