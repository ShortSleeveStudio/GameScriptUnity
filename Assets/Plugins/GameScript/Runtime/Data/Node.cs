using System;

namespace GameScript
{
    [Serializable]
    public class Node
    {
        public uint Id;
        public Actor Actor;
        public Localization UIResponseText;
        public Localization VoiceText;
        public uint Condition; // Index into routine array
        public uint Code; // Index into routine array
        public bool IsPreventResponse;
        public Edge[] OutgoingEdges;
    }
}
