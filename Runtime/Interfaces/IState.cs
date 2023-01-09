namespace com.eyerunnman.patterns
{
    public interface IState<T>
    {
        public void ExecuteStateEnter();
        public void ExecuteStateUpdate();
        public void ExecuteStateExit();
    }

}

