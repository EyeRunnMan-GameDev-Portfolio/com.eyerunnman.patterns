using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.eyerunnman.interfaces
{
    public interface ICommand<T>
    {
        public void Execute(T context);
    }
}

