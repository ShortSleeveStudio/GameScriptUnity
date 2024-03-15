using System.Collections.Generic;

namespace GameScript
{
    /**Called to signal that the conversation runner can proceed.*/
    public delegate void OnReady();

    /**Called to signal that the conversation runner can proceed with the selected node.*/
    public delegate void OnDecisionMade(Node node);

    /**Runner listeners can react to changes in conversation runner state.*/
    public interface RunnerListener
    {
        public void OnConversationEnter(Conversation conversation, OnReady onReady);
        public void OnNodeEnter(Node node, OnReady onReady);
        public void OnNodeDecision(List<Node> nodes, OnDecisionMade onDecisionMade);
        public void OnNodeExit(Node node, OnReady onReady);
        public void OnConversationExit(Conversation conversation, OnReady onReady);
    }
}
