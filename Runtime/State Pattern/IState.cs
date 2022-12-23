using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.eyerunnman.interfaces
{
    public interface IState<T>
    {
        public void OnEnterState(T context);
        public void OnUpdateState(T context);
        public void OnExitState(T context);
        public void SwitchState(T context, IState<T> state);
    }
}

