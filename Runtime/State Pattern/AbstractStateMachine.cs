using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.eyerunnman.patterns
{
    public abstract class AbstractStateMachine<Context, TriggerEnum, StateEnum> : MonoBehaviour where StateEnum : Enum where TriggerEnum : Enum
    {

        protected sealed class StateTreeNode : ITreeNode<StateTreeNode>
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

        protected StateTreeNode StateTree{ get; private set; }

        protected void ComputeStateTree()
        {
            List<AbstractTriggerableState<Context, TriggerEnum, StateEnum>> rootStateObjects = new();

            foreach (StateEnum stateEnum in Enum.GetValues(typeof(StateEnum)))
            {
                var currentStateObj = StateFactory.Create(stateEnum);

                if (currentStateObj is not null &&  currentStateObj.IsRootState)
                {
                    rootStateObjects.Add(currentStateObj);
                }
            }

            List<StateTreeNode> childTreeNodes=new();

            foreach (var item in rootStateObjects)
            {
                childTreeNodes.Add(getStateTreeNode(item));

                StateTreeNode getStateTreeNode(AbstractTriggerableState<Context, TriggerEnum, StateEnum> state)
                {
                    List<AbstractTriggerableState<Context, TriggerEnum, StateEnum>> childStates =new (state.LoadedChildStates.Select(childEnum => StateFactory.Create(childEnum)));

                    if (childStates.Count == 0)
                    {
                        return new(state.StateID,  new List<StateTreeNode>());
                    }
                    else
                    {
                        List <StateTreeNode> childNodeList = new();

                        foreach (var childStateObject in childStates)
                        {
                            StateTreeNode childnode = getStateTreeNode(childStateObject);

                            if (childnode != null) {
                                childNodeList.Add(childnode);
                            }
                        }

                        return new(state.StateID, childNodeList);
                    }
                }
            }

            StateTree = new StateTreeNode(RootEnum, childTreeNodes, true);
        }

        protected abstract StateEnum RootEnum { get;}

        protected internal IFactory<AbstractTriggerableState<Context, TriggerEnum, StateEnum>, StateEnum> StateFactory { get; set; }
        protected internal AbstractTriggerableState<Context, TriggerEnum, StateEnum> CurrentRootState { get; set; }

        protected internal List<StateEnum> CurrentStateChain { get {

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
            } }

        protected internal Context Ctx { get; set; }
        protected internal HashSet<TriggerEnum> CurrentFrameTriggers { get; set; }
    }
}

