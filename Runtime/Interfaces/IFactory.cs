using System;

namespace com.eyerunnman.patterns
{
    public interface IFactory<T>
    {
        public abstract T Create();
    }

    public interface IFactory<T, U> where U : Enum
    {
        public abstract T Create(U type);
    }
}


