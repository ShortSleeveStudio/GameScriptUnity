using System;

namespace GameScript
{
    [Serializable]
    public class Edge
    {
        public uint id;
        public Node source;
        public Node target;
        public long priority;
    }
}
