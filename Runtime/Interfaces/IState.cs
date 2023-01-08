namespace com.eyerunnman.patterns
{
    public interface IState<T>
    {
        public void OnEnterState();

        public void OnUpdateState();

    }

}

