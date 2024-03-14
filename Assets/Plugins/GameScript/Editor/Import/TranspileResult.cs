using System.Collections.Generic;

namespace GameScript
{
    public abstract class ImportResult
    {
        public bool WasError;
    }

    public class DbCodeGeneratorResult : ImportResult
    {
    }

    public class TranspilerResult : ImportResult
    {
        public uint MaxFlags;
        public Dictionary<uint, uint> RoutineIdToIndex;
        public override string ToString() => $"MaxFlags = {MaxFlags}";
    }

    public class ConversationDataGeneratorResult : ImportResult
    {

    }
}
