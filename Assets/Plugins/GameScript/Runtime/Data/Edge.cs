using System;

namespace GameScript
{
    [Serializable]
    public class Edge
    {
        public uint Id;
        public Node Source;
        public Node Target;
        public byte Priority;
    }
}
