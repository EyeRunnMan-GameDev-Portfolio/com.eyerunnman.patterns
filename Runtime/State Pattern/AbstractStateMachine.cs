using System;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractStateMachine<T,U> : MonoBehaviour where U : Enum
    {
        protected internal AbstractState<T,U> CurrentState { get; set; }
        protected internal virtual T Context { get; }
    }

}

