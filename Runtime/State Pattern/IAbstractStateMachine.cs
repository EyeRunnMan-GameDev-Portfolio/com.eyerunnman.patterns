using System;
using System.Collections.Generic;
using System.Linq;

namespace com.eyerunnman.patterns
{
    public interface IAbstractStateMachine<Context, StateEnum, TriggerEnum> where StateEnum : Enum where TriggerEnum : Enum
    {

        public sealed class StateTreeNode : ITreeNode<StateTreeNode>
        {
            public StateEnum StateID { get; private set; }

            public List<StateTreeNode> ChildNodes { get; private set; }

            public bool IsRootNode { get; private set; }
            public bool IsChildNode { get; private set; }
            public bool IsParentNode { get; private set; }

            public StateTreeNode(StateEnum stateId, List<StateTreeNode> childNodes, bool IsRoot = false)
            {
                StateID = stateId;

                if (childNodes.Count == 0)
                {
                    IsParentNode = false;
                }
                else
                {
                    IsParentNode = true;
                    ChildNodes = new(childNodes);
                }

                if (IsRoot)
                {
                    IsRootNode = true;
                    IsChildNode = false;
                }
                else
                {
                    IsChildNode = true;
                }

            }
        }

        public void ComputeStateTree()
        {
            List<AbstractTriggerableState<Context, StateEnum, TriggerEnum>> rootStateObjects = new();

            foreach (StateEnum stateEnum in Enum.GetValues(typeof(StateEnum)))
            {
                var currentStateObj = StateFactory.Create(stateEnum);

                if (currentStateObj is not null && currentStateObj.IsRootState)
                {
                    rootStateObjects.Add(currentStateObj);
                }
            }

            List<StateTreeNode> childTreeNodes=new();

            foreach (var item in rootStateObjects)
            {
                childTreeNodes.Add(getStateTreeNode(item));

                StateTreeNode getStateTreeNode(AbstractTriggerableState<Context, StateEnum, TriggerEnum> state)
                {
                    List<AbstractTriggerableState<Context, StateEnum, TriggerEnum>> childStates =new (state.LoadedChildStates.Select(childEnum => StateFactory.Create(childEnum)));

                    if (childStates.Count == 0)
                    {
                        return new(state.StateID, new List<StateTreeNode>());
                    }
                    else
                    {
                        List <StateTreeNode> childNodeList = new();

                        foreach (var childStateObject in childStates)
                        {
                            StateTreeNode childnode = getStateTreeNode(childStateObject);

                            if (childnode != null)
                            {
                                childNodeList.Add(childnode);
                            }
                        }

                        return new(state.StateID, childNodeList);
                    }
                }
            }

            StateTree = new StateTreeNode(RootEnum, childTreeNodes, true);
        }

        protected internal List<StateEnum> CurrentStateChain
        {
            get
            {

                List <StateEnum> chain = new(){ RootEnum };

                var stateRef= CurrentRootState;

                if (stateRef is not null)
                {
                    chain.Add(stateRef.StateID);
                }

                while (stateRef?.ActiveChildStateNode is not null)
                {
                    chain.Add(stateRef.ActiveChildStateNode.StateID);
                    stateRef = stateRef?.ActiveChildStateNode;
                }

                return chain;
            }
        }


        protected abstract StateEnum RootEnum { get; }
        protected StateTreeNode StateTree { get; set; }
        protected internal IFactory<AbstractTriggerableState<Context, StateEnum, TriggerEnum>, StateEnum> StateFactory { get; }
        protected internal AbstractTriggerableState<Context, StateEnum, TriggerEnum> CurrentRootState { get; set; }
        protected internal Context Ctx { get;}
        protected internal HashSet<TriggerEnum> CurrentFrameTriggers { get; }
    }
}

