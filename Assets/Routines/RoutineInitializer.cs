// GENERATED CODE - DO NOT EDIT BY HAND
using System;
using UnityEngine;

namespace GameScript
{
    public static class RoutineInitializer
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            RoutineDirectory.Directory = new System.Action<ConversationContext>[4];
            RoutineDirectory.Directory[0] = (ConversationContext ctx) =>
            {
            };
            RoutineDirectory.Directory[1] = (ConversationContext ctx) =>
            {
                ctx.SetConditionResult(10<11);
            };
            RoutineDirectory.Directory[2] = (ConversationContext ctx) =>
            {
                ctx.SetBlocksInUse(5);
                {
                    if (!ctx.IsBlockExecuted(0))
                    {
                        Debug.Log("Done");
                        ctx.SetBlockExecuted(0);
                    }
                    if (ctx.HaveBlockSignalsFired(0))
                    {
                        ctx.SetFlag((int)RoutineFlag.Done);
                    }
                }
                if (ctx.IsFlagSet((int)RoutineFlag.Done))
                {
                    if (!ctx.IsBlockExecuted(1))
                    {
                        Debug.Log("Bunzo");
                        ctx.SetBlockExecuted(1);
                    }
                    if (ctx.HaveBlockSignalsFired(1))
                    {
                        ctx.SetFlag((int)RoutineFlag.Bunzo);
                    }
                }
                if (ctx.IsFlagSet((int)RoutineFlag.Bunzo))
                {
                    if (!ctx.IsBlockExecuted(2))
                    {
                        Debug.Log("C1");
                        Helpers.TestNode(ctx.GetCurrentNode());
                        ctx.SetBlockExecuted(2);
                    }
                    if (ctx.HaveBlockSignalsFired(2))
                    {
                        ctx.SetFlag((int)RoutineFlag.C1);
                    }
                }
                if (ctx.IsFlagSet((int)RoutineFlag.Bunzo))
                {
                    if (!ctx.IsBlockExecuted(3))
                    {
                        if(true){Debug.Log("C2");
                        }Helpers.TestSig(ctx.AcquireSignal(3));
                        ctx.SetBlockExecuted(3);
                    }
                    if (ctx.HaveBlockSignalsFired(3))
                    {
                        ctx.SetFlag((int)RoutineFlag.C2);
                    }
                }
                if (ctx.IsFlagSet((int)RoutineFlag.C1) && ctx.IsFlagSet((int)RoutineFlag.C2))
                {
                    if (!ctx.IsBlockExecuted(4))
                    {
                        Debug.Log("FINISHED");
                        ctx.SetBlockExecuted(4);
                    }
                    if (ctx.HaveBlockSignalsFired(4))
                    {
                    }
                }
                
            };
            RoutineDirectory.Directory[3] = (ConversationContext ctx) =>
            {
            };
        }
    }
}
