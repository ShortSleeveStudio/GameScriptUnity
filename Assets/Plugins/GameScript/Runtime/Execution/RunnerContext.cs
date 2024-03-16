using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameScript
{
    public class RunnerContext
    {
        #region Constants
        private const int k_DefaultEdgeCapacity = 16;
        #endregion

        public uint ContextId { get; private set; }

        private static uint s_NextContextId = 0;
        private RunnerRoutineState m_RoutineState;
        private Conversation m_Conversation;
        private Node m_Node;
        private IRunnerListener m_Listener;
        private bool m_OnReadyCalled;
        private OnReady m_OnReady;
        private Node m_OnDecisionMadeValue;
        private bool m_OnDecisionMadeCalled;
        private OnDecisionMade m_OnDecisionMade;
        private MachineState m_CurrentState;
        private List<Node> m_AvailableNodes;

        public RunnerContext(Settings settings)
        {
            m_OnReady = OnReady;
            ContextId = s_NextContextId;
            m_CurrentState = MachineState.Idle;
            m_OnDecisionMade = OnDecisionMade;
            m_RoutineState = new(settings.MaxFlags);
            m_AvailableNodes = new(k_DefaultEdgeCapacity);
        }

        #region Execution
        public void Start(Conversation conversation, IRunnerListener listener)
        {
            m_Node = conversation.RootNode;
            m_Listener = listener;
            m_Conversation = conversation;
            m_CurrentState = MachineState.ConversationEnter;
        }

        public void Stop()
        {
            // Since this is only called by Runner, we don't necessarily have to go though
            // OnConversationExit. Runner will already place this context back into the inactive
            // list.
            // The only question is: will users want to receive OnConversationExit?
            Reset();
        }

        /**Returns if the conversation is active*/
        public bool Tick()
        {
            try
            {
                switch (m_CurrentState)
                {
                    case MachineState.Idle:
                    {
                        return false;
                    }
                    case MachineState.ConversationEnter:
                    {
                        m_Listener.OnConversationEnter(m_Conversation, m_OnReady);
                        m_CurrentState = MachineState.ConversationEnterWait;
                        goto case MachineState.ConversationEnterWait;
                    }
                    case MachineState.ConversationEnterWait:
                    {
                        if (!m_OnReadyCalled)
                            return true;
                        m_OnReadyCalled = false;
                        m_CurrentState = MachineState.NodeEnter;
                        goto case MachineState.NodeEnter;
                    }
                    case MachineState.NodeEnter:
                    {
                        m_Listener.OnNodeEnter(m_Node, m_OnReady);
                        m_CurrentState = MachineState.NodeEnterWait;
                        goto case MachineState.NodeEnterWait;
                    }
                    case MachineState.NodeEnterWait:
                    {
                        if (!m_OnReadyCalled)
                            return true;
                        m_OnReadyCalled = false;
                        m_CurrentState = MachineState.NodeExecute;
                        goto case MachineState.NodeExecute;
                    }
                    case MachineState.NodeExecute:
                    {
                        RoutineDirectory.Directory[m_Node.Code](this);
                        if (!m_RoutineState.IsRoutineExecuted())
                            return true;
                        m_RoutineState.Reset();
                        m_CurrentState = MachineState.NodeExit;
                        goto case MachineState.NodeExit;
                    }
                    case MachineState.NodeExit:
                    {
                        m_Listener.OnNodeExit(m_Node, m_OnReady);
                        m_CurrentState = MachineState.NodeExitWait;
                        goto case MachineState.NodeExitWait;
                    }
                    case MachineState.NodeExitWait:
                    {
                        if (!m_OnReadyCalled)
                            return true;
                        m_OnReadyCalled = false;
                        m_CurrentState = MachineState.NodeDecision;
                        goto case MachineState.NodeDecision;
                    }
                    case MachineState.NodeDecision:
                    {
                        // Gather available edges
                        uint actorId = 0;
                        bool allEdgesSameActor = true;
                        byte priority = 0;
                        Node highestPriorityNode = null;
                        for (uint i = 0; i < m_Node.OutgoingEdges.Length; i++)
                        {
                            Edge edge = m_Node.OutgoingEdges[i];
                            // Conditions cannot be async
                            RoutineDirectory.Directory[edge.Target.Condition](this);
                            if (m_RoutineState.GetConditionResult())
                            {
                                // Retain a list of viable nodes
                                m_AvailableNodes.Add(edge.Target);

                                // See if all actors are the same
                                if (m_AvailableNodes.Count == 1)
                                    actorId = edge.Target.Actor.Id;
                                else if (allEdgesSameActor && actorId != edge.Target.Actor.Id)
                                {
                                    allEdgesSameActor = false;
                                }

                                // Track highest priority node
                                if (highestPriorityNode == null || priority < edge.Priority)
                                {
                                    priority = edge.Priority;
                                    highestPriorityNode = edge.Target;
                                }
                            }
                            m_RoutineState.Reset();
                        }

                        // Conversation Exit - No Available Edges
                        if (m_AvailableNodes.Count == 0)
                        {
                            m_Listener.OnConversationExit(m_Conversation, m_OnReady);
                            m_CurrentState = MachineState.ConversationExitWait;
                            goto case MachineState.ConversationExitWait;
                        }

                        // Node Decision
                        if (
                            m_AvailableNodes.Count > 1
                            && allEdgesSameActor
                            && !m_Node.IsPreventResponse
                        )
                        {
                            m_Listener.OnNodeDecision(m_AvailableNodes, m_OnDecisionMade);
                            m_CurrentState = MachineState.NodeDecisionWait;
                            goto case MachineState.NodeDecisionWait;
                        }

                        // Node Enter
                        m_AvailableNodes.Clear();
                        m_Node = highestPriorityNode;
                        m_CurrentState = MachineState.NodeEnter;
                        goto case MachineState.NodeEnter;
                    }
                    case MachineState.NodeDecisionWait:
                    {
                        if (!m_OnDecisionMadeCalled)
                            return true;
                        m_Node = m_OnDecisionMadeValue;
                        m_AvailableNodes.Clear();
                        m_OnDecisionMadeCalled = false;
                        m_OnDecisionMadeValue = null;
                        m_CurrentState = MachineState.NodeEnter;
                        goto case MachineState.NodeEnter;
                    }
                    case MachineState.ConversationExitWait:
                    {
                        if (!m_OnReadyCalled)
                            return true;
                        return false; // Reset is called by Runner
                    }
                    default:
                    {
                        throw new Exception($"Invalid state machine state {m_CurrentState}");
                    }
                }
            }
            catch (Exception e)
            {
                m_Listener.OnError(m_Conversation, e);
                return false; // Reset is called by Runner
            }
        }

        private void OnReady() => m_OnReadyCalled = true;

        private void OnDecisionMade(Node node)
        {
            m_OnDecisionMadeValue = node;
            m_OnDecisionMadeCalled = true;
        }

        private void Reset()
        {
            m_Node = null;
            m_Listener = null;
            m_Conversation = null;
            m_CurrentState = MachineState.Idle;
            m_RoutineState.Reset();
            m_OnReadyCalled = false;
            m_AvailableNodes.Clear();
            m_OnDecisionMadeValue = null;
            m_OnDecisionMadeCalled = false;
        }
        #endregion

        #region Conversation
        public Conversation GetCurrentConversation() => m_Conversation;

        public void SetCurrentConversation(Conversation conversation) =>
            m_Conversation = conversation;

        public Node GetCurrentNode() => m_Node;

        public void SetCurrentNode(Node node) => m_Node = node;
        #endregion

        #region Routines
        public void SetConditionResult(bool result) => m_RoutineState.SetConditionResult(result);
        #endregion

        #region Scheduled Blocks
        public void SetBlocksInUse(int blockCount) => m_RoutineState.SetBlocksInUse(blockCount);

        public bool IsBlockExecuted(int blockIndex) => m_RoutineState.IsBlockExecuted(blockIndex);

        public void SetBlockExecuted(int blockIndex) => m_RoutineState.SetBlockExecuted(blockIndex);

        public Signal AcquireSignal(int blockIndex) => m_RoutineState.AcquireSignal(blockIndex);

        public bool HaveBlockSignalsFired(int blockIndex) =>
            m_RoutineState.HaveBlockSignalsFired(blockIndex);
        #endregion

        #region Flags
        public void SetFlag(int flagIndex) => m_RoutineState.SetFlag(flagIndex);

        public bool IsFlagSet(int flagIndex) => m_RoutineState.IsFlagSet(flagIndex);
        #endregion

        #region States
        private enum MachineState
        {
            Idle,
            ConversationEnter,
            ConversationEnterWait,
            NodeEnter,
            NodeEnterWait,
            NodeExecute,
            NodeExit,
            NodeExitWait,
            NodeDecision,
            NodeDecisionWait,
            ConversationExit,
            ConversationExitWait,
        }
        #endregion
    }
}
