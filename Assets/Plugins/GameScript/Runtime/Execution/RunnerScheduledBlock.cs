using System.Collections.Generic;

namespace GameScript
{
    class RunnerScheduledBlock : ILessor
    {
        private const int k_InitialSignalPool = 8; // Conservative guess
        private List<SignalData> m_Signals;
        private int m_CurrentSignal;
        private bool m_Executed;
        private RunnerContext m_RunnerContext;

        public RunnerScheduledBlock(RunnerContext runnerContext)
        {
            m_RunnerContext = runnerContext;
            m_Signals = new(k_InitialSignalPool);
            EnsurePoolSize(k_InitialSignalPool);

            m_Executed = false;
            m_CurrentSignal = 0;
        }

        #region ILessor
        public Lease Acquire()
        {
            int currentSignal = m_CurrentSignal++;
            EnsurePoolSize(m_CurrentSignal);
            return new(
                m_RunnerContext.SequenceNumber,
                m_RunnerContext,
                m_Signals[currentSignal].Signal
            );
        }
        #endregion

        public bool HasExecuted() => m_Executed;

        public void SetExecuted() => m_Executed = true;

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
                    new() { Signal = () => m_Signals[index].Triggered = true, Triggered = false, }
                );
            }
        }

        private class SignalData
        {
            public Signal Signal;
            public bool Triggered;
        }
    }
}
