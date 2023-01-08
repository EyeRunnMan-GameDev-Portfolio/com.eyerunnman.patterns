using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public interface IProxy<T>
    {
        public void ExecuteCommand(ICommand<T> command);
    }

}


