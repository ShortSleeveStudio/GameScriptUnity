using System;

namespace GameScript
{
    [Serializable]
    public class Conversation
    {
        public uint id;
        public string name;
        public Node rootNode;
        public Node[] nodes;
    }
}
