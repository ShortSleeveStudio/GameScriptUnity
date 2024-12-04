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

        public void Stop() => GameScriptRunner.StopConversation(this);

        public bool IsActive() => GameScriptRunner.IsActive(this);

        public void RegisterFlagListener(Action<int> listener) =>
            GameScriptRunner.RegisterFlagListener(this, listener);

        public void UnregisterFlagListener(Action<int> listener) =>
            GameScriptRunner.UnregisterFlagListener(this, listener);

        public void SetFlag(int flag) => GameScriptRunner.SetFlag(this, flag);
    }
}
