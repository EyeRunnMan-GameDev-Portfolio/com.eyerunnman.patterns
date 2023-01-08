using com.eyerunnman.patterns;
using System;

namespace com.eyerunnman.patterns
{
    public abstract class RootAbstractTriggerableState<Context, TriggerEnums, StateEnum> : AbstractTriggerableState<Context, TriggerEnums, StateEnum> where TriggerEnums : Enum where StateEnum : Enum
    {
        public RootAbstractTriggerableState(AbstractStateMachine<Context, TriggerEnums, StateEnum> context, StateEnum stateID) :base(context,stateID)
        {

        }

        protected override bool IsRootState => true;
    }
}


