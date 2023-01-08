using System;

namespace com.eyerunnman.patterns
{
    public interface ITriggerable<T> where T: Enum
    {
        public void OnTrigger(T TriggerEnum);
    }

}

