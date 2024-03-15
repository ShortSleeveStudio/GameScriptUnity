using System;

namespace GameScript
{
    [Serializable]
    public class Conversation : IComparable<Conversation>
    {
        public uint Id;
        public string Name;
        public Node RootNode;
        public Node[] Nodes;

        public int CompareTo(Conversation other) => Id.CompareTo(other.Id);
    }
}
