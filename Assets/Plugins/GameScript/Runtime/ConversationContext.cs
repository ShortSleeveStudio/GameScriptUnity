using System;

namespace GameScript
{
    public class ConversationContext
    {
        private const int MAX_BLOCKS = 64;
        private const int MAX_SIGNALS = 64;

        private RoutineState m_RoutineState;
        private Conversation m_Conversation;
        private ConversationNode m_Node;

        public ConversationContext(Type flagType)
        {
            m_RoutineState = new(Enum.GetValues(flagType).Length);
        }

        #region Conversation
        public ConversationNode GetCurrentNode() => m_Node;
        public void SetCurrentNode(ConversationNode node) => m_Node = node;
        public Conversation GetCurrentConversation() => m_Conversation;
        public void SetCurrentConversation(Conversation conversation)
            => m_Conversation = conversation;
        #endregion

        #region Routines
        public bool IsRoutineExecuted() => m_RoutineState.IsRoutineExecuted();
        public void ResetRoutineState() => m_RoutineState.Reset();
        public bool GetConditionResult() => m_RoutineState.GetConditionResult();
        public void SetConditionResult(bool result) => m_RoutineState.SetConditionResult(result);
        #endregion

        #region Scheduled Blocks
        public void SetBlocksInUse(int blockCount) => m_RoutineState.SetBlocksInUse(blockCount);
        public bool IsBlockExecuted(int blockIndex) => m_RoutineState.IsBlockExecuted(blockIndex);
        public void SetBlockExecuted(int blockIndex) => m_RoutineState.SetBlockExecuted(blockIndex);
        public Signal AcquireSignal(int blockIndex) => m_RoutineState.AcquireSignal(blockIndex);
        #endregion

        #region Flags
        public void SetFlag(int flagIndex) => m_RoutineState.SetFlag(flagIndex);
        public bool IsFlagSet(int flagIndex) => m_RoutineState.IsFlagSet(flagIndex);
        #endregion

        #region Helper Classes
        class RoutineState
        {
            private int m_BlocksInUse;
            private ScheduledBlock[] m_Blocks;
            private bool[] m_FlagState;
            private bool m_IsCondition;
            private bool m_ConditionResult;

            public RoutineState(int flagCount)
            {
                m_Blocks = new ScheduledBlock[MAX_BLOCKS];
                m_BlocksInUse = 0;
                m_FlagState = new bool[flagCount];
                m_IsCondition = false;
                m_ConditionResult = false;
            }

            public bool IsRoutineExecuted()
            {
                for (int i = 0; i < m_BlocksInUse; i++)
                {
                    ScheduledBlock block = m_Blocks[i];
                    if (!block.HasExecuted()) return false;
                    if (!block.HaveAllSignalsFired()) return false;
                }
                return true;
            }

            public void SetBlocksInUse(int blockCount) => m_BlocksInUse = blockCount;
            public bool IsBlockExecuted(int blockIndex) => m_Blocks[blockIndex].HasExecuted();
            public void SetBlockExecuted(int blockIndex) => m_Blocks[blockIndex].SetExecuted();
            public Signal AcquireSignal(int blockIndex) => m_Blocks[blockIndex].AcquireSignal();

            public bool GetConditionResult()
            {
                if (!m_IsCondition)
                {
                    throw new Exception(
                        "Tried to access condition result from a non-condition routine");
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
                for (int i = 0; i < m_BlocksInUse; i++) m_Blocks[i].Reset();
                for (int i = 0; i < m_FlagState.Length; i++) m_FlagState[i] = false;
            }
        }

        class ScheduledBlock
        {
            private Signal[] m_Signals;
            private bool[] m_SignalState;
            private int m_CurrentSignal;
            private bool m_Executed;

            public ScheduledBlock()
            {
                m_Signals = new Signal[MAX_SIGNALS];
                m_Executed = false;
                m_SignalState = new bool[MAX_SIGNALS];
                m_CurrentSignal = 0;
                for (int i = 0; i < MAX_SIGNALS; i++) m_Signals[i] = () => m_SignalState[i] = true;
            }

            public Signal AcquireSignal() => m_Signals[m_CurrentSignal++];

            public bool HasExecuted() => m_Executed;
            public void SetExecuted() => m_Executed = true;

            public bool HaveAllSignalsFired()
            {
                for (int i = 0; i < m_CurrentSignal; i++)
                {
                    if (!m_SignalState[i]) return false;
                }
                return true;
            }

            public void Reset()
            {
                m_Executed = false;
                m_CurrentSignal = 0;
                for (int i = 0; i < MAX_SIGNALS; i++) m_SignalState[i] = false;
            }
        }
        #endregion
    }
}
