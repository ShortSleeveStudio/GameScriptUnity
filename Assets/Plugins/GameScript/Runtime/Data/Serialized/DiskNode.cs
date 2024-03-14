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
        // IDs are remapped to be sequential
        public uint condition;
        // IDs are remapped to be sequential
        public uint code;
        public bool isPreventResponse;
        public bool isRoot;
    }
}
