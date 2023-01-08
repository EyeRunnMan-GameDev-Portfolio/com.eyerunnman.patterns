using com.eyerunnman.patterns;
using System;

namespace com.eyerunnman.patterns
{
    public abstract class RootAbstractState<T,U> : AbstractState<T,U> where U: Enum
    {
        public RootAbstractState(AbstractStateMachine<T,U> stateContext):base(stateContext)
        {

        }

        protected override bool IsRootState => true;
    }
}


