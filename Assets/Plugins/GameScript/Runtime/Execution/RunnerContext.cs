using System;
using System.Collections.Generic;

namespace GameScript
{
    public class RunnerContext
    {
        #region Constants
        private const int k_DefaultEdgeCapacity = 16;
        #endregion

        public uint ContextId { get; private set; }

        private static uint s_NextContextId = 0;
        private RoutineState m_RoutineState;
        private Conversation m_Conversation;
        private Node m_Node;
        private RunnerListener m_Listener;
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
        public void Start(Conversation conversation, RunnerListener listener)
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

                    // Conversation Exit - No Edges
                    if (m_AvailableNodes.Count == 0)
                    {
                        m_Listener.OnConversationExit(m_Conversation, m_OnReady);
                        m_CurrentState = MachineState.ConversationExitWait;
                        goto case MachineState.ConversationExitWait;
                    }

                    // Node Decision
                    if (allEdgesSameActor && !m_Node.IsPreventResponse)
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
                    Reset();
                    return false;
                }
                default:
                {
                    throw new Exception($"Invalid state machine state {m_CurrentState}");
                }
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

        #region Helper Classes
        class RoutineState
        {
            private const int k_InitialBlockPool = 8; // Conservative guess
            private int m_BlocksInUse;
            private List<ScheduledBlock> m_Blocks;
            private bool[] m_FlagState;
            private bool m_IsCondition;
            private bool m_ConditionResult;

            public RoutineState(uint maxFlags)
            {
                m_Blocks = new(k_InitialBlockPool);
                EnsurePoolSize(k_InitialBlockPool);
                m_BlocksInUse = 0;
                m_FlagState = new bool[maxFlags];
                m_IsCondition = false;
                m_ConditionResult = false;
            }

            public bool IsRoutineExecuted()
            {
                for (int i = 0; i < m_BlocksInUse; i++)
                {
                    ScheduledBlock block = m_Blocks[i];
                    if (!block.HasExecuted())
                        return false;
                    if (!block.HaveAllSignalsFired())
                        return false;
                }
                return true;
            }

            public void SetBlocksInUse(int blockCount)
            {
                m_BlocksInUse = blockCount;
                EnsurePoolSize(m_BlocksInUse);
            }

            public bool IsBlockExecuted(int blockIndex) => m_Blocks[blockIndex].HasExecuted();

            public void SetBlockExecuted(int blockIndex) => m_Blocks[blockIndex].SetExecuted();

            public Signal AcquireSignal(int blockIndex) => m_Blocks[blockIndex].AcquireSignal();

            public bool HaveBlockSignalsFired(int blockIndex) =>
                m_Blocks[blockIndex].HaveAllSignalsFired();

            public bool GetConditionResult()
            {
                if (!m_IsCondition)
                {
                    throw new Exception(
                        "Tried to access condition result from a non-condition routine"
                    );
                }
                return m_ConditionResult;
            }

            public void SetConditionResult(bool result)
            {
                m_IsCondition = true;
                m_ConditionResult = result;
            }

            public void SetFlag(int flagIndex) => m_FlagState[flagIndex] = true;

            public bool IsFlagSet(int flagIndex) => m_FlagState[flagIndex];

            public void Reset()
            {
                m_BlocksInUse = 0;
                m_IsCondition = false;
                m_ConditionResult = false;
                for (int i = 0; i < m_BlocksInUse; i++)
                    m_Blocks[i].Reset();
                for (int i = 0; i < m_FlagState.Length; i++)
                    m_FlagState[i] = false;
            }

            private void EnsurePoolSize(int poolSize)
            {
                for (int i = m_Blocks.Count; i < poolSize; i++)
                    m_Blocks.Add(new());
            }
        }

        class ScheduledBlock
        {
            private const int k_InitialSignalPool = 8; // Conservative guess
            private List<SignalData> m_Signals;
            private int m_CurrentSignal;
            private bool m_Executed;

            public ScheduledBlock()
            {
                m_Signals = new(k_InitialSignalPool);
                EnsurePoolSize(k_InitialSignalPool);

                m_Executed = false;
                m_CurrentSignal = 0;
            }

            public bool HasExecuted() => m_Executed;

            public void SetExecuted() => m_Executed = true;

            public Signal AcquireSignal()
            {
                int currentSignal = m_CurrentSignal++;
                EnsurePoolSize(m_CurrentSignal);
                return m_Signals[currentSignal].Signal;
            }

            public bool HaveAllSignalsFired()
            {
                for (int i = 0; i < m_CurrentSignal; i++)
                {
                    if (!m_Signals[i].Triggered)
                        return false;
                }
                return true;
            }

            public void Reset()
            {
                m_Executed = false;
                m_CurrentSignal = 0;
                for (int i = 0; i < m_Signals.Count; i++)
                    m_Signals[i].Triggered = false;
            }

            private void EnsurePoolSize(int poolSize)
            {
                for (int i = m_Signals.Count; i < poolSize; i++)
                {
                    // Capture
                    int index = i;
                    m_Signals.Add(
                        new()
                        {
                            Signal = () => m_Signals[index].Triggered = true,
                            Triggered = false,
                        }
                    );
                }
            }

            private class SignalData
            {
                public Signal Signal;
                public bool Triggered;
            }
        }
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
