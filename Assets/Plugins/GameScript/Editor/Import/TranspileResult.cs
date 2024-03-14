using System.Collections.Generic;

namespace GameScript
{
    public class TranspilerResult
    {
        public uint MaxFlags;
        public uint NoopRoutineId;
        public Dictionary<uint, uint> RoutineIdToIndex;
        public override string ToString() => $"MaxFlags = {MaxFlags}";
    }
}
