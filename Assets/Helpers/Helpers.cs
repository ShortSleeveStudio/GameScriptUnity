using System.Threading.Tasks;
using GameScript;
using UnityEngine;

public class Helpers
{
    public static void TestNode(ConversationNode node)
    {
        Debug.Log("Test Node Called " + node);
    }

    public async static void TestSig(Signal signal)
    {
        Debug.Log("Test Signal Called");
        await Task.Delay(2000);
        Debug.Log("Test Signal Completed");
        signal();
    }
}
