namespace GameScript
{
    public static class EditorConstants
    {
        public const int k_SqlBatchSize = 500;
        public const string k_RoutineTypesTableName = "routine_types";
        public const string k_GeneratedCodeWarning = "GENERATED CODE - DO NOT EDIT BY HAND";
        public const string k_ContextClass = "RunnerContext";
        public const string k_RoutineDirectoryClass = "RoutineDirectory";
        public const string k_RoutineInitializerClass = "RoutineInitializer";
        public const string k_RoutineFlagEnum = "RoutineFlag";
        public const uint k_NoopRoutineCodeId = uint.MaxValue;
        public const uint k_NoopRoutineConditionId = uint.MaxValue - 1;
    }
}
