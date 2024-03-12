// GENERATED CODE - DO NOT EDIT BY HAND
using System;
using UnityEngine;

namespace GameScript
{
    public static class RoutineDirectory
    {
        public static System.Action<ConversationContext>[] Directory = new System.Action<ConversationContext>[4];
        static RoutineDirectory()
        {
            Directory[0] = (ConversationContext ctx) =>
            {
            };
            Directory[1] = (ConversationContext ctx) =>
            {
                ctx.SetConditionResult(10<11);

            };
            Directory[2] = (ConversationContext ctx) =>
            {
                ctx.SetBlocksInUse(5);
                if (!ctx.IsBlockExecuted(0))
                {
                    Debug.Log("Done");
                    ctx.SetBlockExecuted(0);
                    ctx.SetFlag((int)RoutineFlag.Done);
                }
                if (!ctx.IsBlockExecuted(1) && ctx.IsFlagSet((int)RoutineFlag.Done))
                {
                    Debug.Log("Bunzo");
                    ctx.SetBlockExecuted(1);
                    ctx.SetFlag((int)RoutineFlag.Bunzo);
                }
                if (!ctx.IsBlockExecuted(2) && ctx.IsFlagSet((int)RoutineFlag.Bunzo))
                {
                    Debug.Log("C1");
                    Helpers.TestNode(ctx.GetCurrentNode());
                    ctx.SetBlockExecuted(2);
                    ctx.SetFlag((int)RoutineFlag.C1);
                }
                if (!ctx.IsBlockExecuted(3) && ctx.IsFlagSet((int)RoutineFlag.Bunzo))
                {
                    Debug.Log("C2");
                    Helpers.TestSig(ctx.AcquireSignal(3));
                    ctx.SetBlockExecuted(3);
                    ctx.SetFlag((int)RoutineFlag.C2);
                }
                if (!ctx.IsBlockExecuted(4) && ctx.IsFlagSet((int)RoutineFlag.C1) && ctx.IsFlagSet((int)RoutineFlag.C2))
                {
                    Debug.Log("FINISHED");
                    ctx.SetBlockExecuted(4);
                }
                

            };
            Directory[3] = (ConversationContext ctx) =>
            {
            };
        }
    }
}
