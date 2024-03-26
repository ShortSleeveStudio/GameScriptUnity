using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameScript
{
    public static class Helpers
    {
        public static Dictionary<string, bool> S = new();

        public static async void TestLease(Lease lease, int millis = 1000)
        {
            Debug.Log($"Waiting {millis} milliseconds");
            await Task.Delay(millis);
            Debug.Log("Done waiting, let's do this!");
            lease.Release();
        }

        public static void TestNode(Node currentNode)
        {
            Debug.Log($"Current Node: {currentNode.Id}");
        }
    }
}
