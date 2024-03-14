using System;

namespace GameScript
{
    [Serializable]
    public class Node
    {
        public uint id;
        public uint actor;
        public Localization uiResponseText;
        public Localization voiceText;
        public uint condition; // Index into routine array
        public uint code; // Index into routine array
        public bool isPreventResponse;
        public Edge[] outgoingEdges;
    }
}
