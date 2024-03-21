using System.Threading.Tasks;
using UnityEngine;

namespace GameScript
{
    public static class Helpers
    {
        public static async void TestLease(Lease lease, int millis)
        {
            Debug.Log($"Waiting {millis} milliseconds");
            await Task.Delay(millis);
            Debug.Log("Done waiting, let's do this!");
        }

        public static void TestNode(Node currentNode)
        {
            Debug.Log($"Current Node: {currentNode.Id}");
        }
    }
}
