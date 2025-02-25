using System;

namespace GameScript
{
    public struct ActiveConversation
    {
        internal GameScriptRunner Runner;
        internal uint SequenceNumber;
        internal uint ContextId;

        internal ActiveConversation(
            GameScriptRunner runner,
            uint sequenceToken,
            uint cancellationToken
        )
        {
            Runner = runner;
            SequenceNumber = sequenceToken;
            ContextId = cancellationToken;
        }

        public void Stop() => Runner.StopConversation(this);

        public bool IsActive() => Runner.IsActive(this);

        public void RegisterFlagListener(Action<int> listener) =>
            Runner.RegisterFlagListener(this, listener);

        public void UnregisterFlagListener(Action<int> listener) =>
            Runner.UnregisterFlagListener(this, listener);

        public void SetFlag(int flag) => Runner.SetFlag(this, flag);
    }
}
