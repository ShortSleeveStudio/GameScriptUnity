// GENERATED CODE - DO NOT EDIT BY HAND
using System;

namespace GameScript
{
    public static class RoutineDirectory
    {
        public static System.Action[] Directory = new System.Action[5];
        static RoutineDirectory()
        {
            Directory[1] = () =>
            {
                /* Code Goes Here
                Debug.Log("Testing");
                */
            };
            Directory[2] = () =>
            {
                /* Code Goes Here
                10 < 11
                */
            };
            Directory[3] = () =>
            {
                /* Code Goes Here
                <-
    Debug.Log("Done");
-Done>

<Done-
    Debug.Log("Bunzo");
-Bunzo>

<Bunzo-
    Debug.Log("C1");
-C1>

<Bunzo-
    Debug.Log("C2");
-C2>

<C1,C2-
    Debug.Log("FINISHED");
->
                */
            };
            Directory[4] = () =>
            {
                /* Code Goes Here
                
                */
            };
        }
    }
}
