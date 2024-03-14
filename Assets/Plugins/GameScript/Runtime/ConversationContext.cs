using System;
using System.Collections.Generic;

namespace GameScript
{
    public class ConversationContext
    {
        private RoutineState m_RoutineState;
        private GameConversation m_Conversation;
        private GameNode m_Node;

        public ConversationContext(Settings settings)
        {
            m_RoutineState = new(settings.MaxFlags);
        }

        #region Conversation
        public GameConversation GetCurrentConversation() => m_Conversation;
        public void SetCurrentConversation(GameConversation conversation)
            => m_Conversation = conversation;
        public GameNode GetCurrentNode() => m_Node;
        public void SetCurrentNode(GameNode node) => m_Node = node;
        #endregion

        #region Execution

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
        public bool HaveBlockSignalsFired(int blockIndex)
            => m_RoutineState.HaveBlockSignalsFired(blockIndex);
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
                    if (!block.HasExecuted()) return false;
                    if (!block.HaveAllSignalsFired()) return false;
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
            public bool HaveBlockSignalsFired(int blockIndex)
                => m_Blocks[blockIndex].HaveAllSignalsFired();

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

            private void EnsurePoolSize(int poolSize)
            {
                for (int i = m_Blocks.Count; i < poolSize; i++) m_Blocks.Add(new());
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
                    if (!m_Signals[i].Triggered) return false;
                }
                return true;
            }

            public void Reset()
            {
                m_Executed = false;
                m_CurrentSignal = 0;
                for (int i = 0; i < m_Signals.Count; i++) m_Signals[i].Triggered = false;
            }

            private void EnsurePoolSize(int poolSize)
            {
                for (int i = m_Signals.Count; i < poolSize; i++)
                {
                    // Capture
                    int index = i;
                    m_Signals.Add(new()
                    {
                        Signal = () => m_Signals[index].Triggered = true,
                        Triggered = false,
                    });
                }
            }

            private class SignalData
            {
                public Signal Signal;
                public bool Triggered;
            }
        }
        #endregion
    }
}
