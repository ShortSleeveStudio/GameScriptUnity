using System;

namespace GameScript
{
    public struct ActiveConversation
    {
        internal uint SequenceNumber;
        internal uint ContextId;

        internal ActiveConversation(uint sequenceToken, uint cancellationToken)
        {
            SequenceNumber = sequenceToken;
            ContextId = cancellationToken;
        }

        public void Stop() => Runner.StopConversation(this);

        public bool IsActive() => Runner.IsActive(this);

        public void RegisterFlagListener(Action<RoutineFlag> listener) =>
            Runner.RegisterFlagListener(this, listener);

        public void UnregisterFlagListener(Action<RoutineFlag> listener) =>
            Runner.UnregisterFlagListener(this, listener);

        public void SetFlag(RoutineFlag flag) => Runner.SetFlag(this, flag);
    }
}
