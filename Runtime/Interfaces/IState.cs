using System.Collections.Generic;

namespace com.eyerunnman.patterns
{
    public interface IState<T>
    {
        public void ExecuteStateEnter();
        public void ExecuteStateUpdate();
        public void ExecuteStateExit();
    }

    public interface ITreeNode<T>
    {
        public List<T> ChildNodes { get; }

        public bool IsRootNode { get; }
        public bool IsParentNode { get; }
        public bool IsChildNode { get; }

    }

}

