using System;

namespace GameScript
{
    [Serializable]
    public class DiskConversation
    {
        public uint id;
        public string name;
        public DiskNode[] diskNodes;
        public DiskEdge[] diskEdges;
    }
}
