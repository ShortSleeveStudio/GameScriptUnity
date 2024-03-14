using System;

namespace GameScript
{
    [Serializable]
    public class DiskNode
    {
        public uint id;
        public uint actor;
        public DiskLocalization uiResponseText;
        public DiskLocalization voiceText;
        public uint condition; // Index into routine array
        public uint code; // Index into routine array
        public bool isPreventResponse;
        public bool isRoot;
    }
}
