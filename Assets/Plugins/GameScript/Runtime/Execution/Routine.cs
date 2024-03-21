using System;

namespace GameScript
{
    public interface IRoutine
    {
        public void Execute(RunnerContext ctx);
    }

    public class NoopBlockRoutine : IRoutine
    {
        public void Execute(RunnerContext ctx) { }
    }

    public class NoopConditionRoutine : IRoutine
    {
        public void Execute(RunnerContext ctx)
        {
            ctx.SetConditionResult(true);
        }
    }

    public class BlockRoutine : IRoutine
    {
        private Action m_Block;

        public BlockRoutine(Action block) => m_Block = block;

        public void Execute(RunnerContext ctx) => m_Block();
    }

    public class ScheduledBlockRoutine : IRoutine
    {
        private ScheduledBlockData[] m_Blocks;

        public ScheduledBlockRoutine(params ScheduledBlockData[] blocks)
        {
            m_Blocks = blocks;
        }

        public void Execute(RunnerContext ctx)
        {
            uint seq = ctx.SequenceNumber;
            ctx.SetBlocksInUse(m_Blocks.Length);
            for (int i = 0; i < m_Blocks.Length; i++)
            {
                ScheduledBlockData data = m_Blocks[i];
                // If the entry flags are set and we haven't already completed this code, execute
                if (!ctx.HaveBlockFlagsFired(i) && ctx.AreFlagsSet(data.FlagsIn))
                {
                    // If we haven't already executed the code block, go for it now
                    if (!ctx.IsBlockExecuted(i))
                    {
                        data.Code(ctx, seq);
                        // In case the conversation was stopped during the routine
                        if (ctx.SequenceNumber != seq)
                            return;
                        ctx.SetBlockExecuted(i);
                    }

                    // If all leases have been released, fire exit flags and mark this block completed
                    if (ctx.HaveBlockSignalsFired(i))
                    {
                        ctx.SetFlags(data.FlagsOut);
                        ctx.SetBlockFlagsFired(i);
                    }
                }
            }
        }
    }

    public class ConditionRoutine : IRoutine
    {
        private Func<bool> m_Condition;

        public ConditionRoutine(Func<bool> condition) => m_Condition = condition;

        public void Execute(RunnerContext ctx) => ctx.SetConditionResult(m_Condition());
    }

    public struct ScheduledBlockData
    {
        internal RoutineFlag[] FlagsIn;
        internal Action<RunnerContext, uint> Code;
        internal RoutineFlag[] FlagsOut;

        public ScheduledBlockData(
            RoutineFlag[] flagsIn,
            Action<RunnerContext, uint> code,
            RoutineFlag[] flagsOut
        )
        {
            FlagsIn = flagsIn;
            Code = code;
            FlagsOut = flagsOut;
        }
    }
}
