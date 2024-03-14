using System;

namespace GameScript
{
    [Serializable]
    public class DiskEdge
    {
        public uint id;
        public uint source;
        public uint target;
        public long priority;
    }
}
