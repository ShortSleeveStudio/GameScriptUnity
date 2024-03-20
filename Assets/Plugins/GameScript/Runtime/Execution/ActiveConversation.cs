namespace GameScript
{
    public struct ActiveConversation
    {
        internal uint SequenceNumber;
        internal uint CancellationToken;

        internal ActiveConversation(uint sequenceToken, uint cancellationToken)
        {
            SequenceNumber = sequenceToken;
            CancellationToken = cancellationToken;
        }

        public void Stop()
        {
            Runner.StopConversation(this);
        }
    }
}
