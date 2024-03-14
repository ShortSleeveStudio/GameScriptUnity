using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEditor;
using System.IO;
using static GameScript.StringWriter;
using static GameScript.Database;

namespace GameScript
{
    public class Transpiler
    {
        #region API
        /**
         * This will build an array of Action's that will be looked up by index. The final index
         * contains the "noop" Action used for all empty/null routines.
         */
        public static TranspilerResult Transpile(
            string sqliteDatabasePath, string routineOutputDirectory, string flagOutputDirectory)
        {
            // Create flag cache
            TranspilerResult transpilerResult = new TranspilerResult();
            HashSet<string> flagCache = new();
            Dictionary<uint, uint> routineIdToIndex = new();
            string importString = "";
            string routinePath = null;

            int progressId = 0;
            try
            {
                // Start progress tracking
                progressId = Progress.Start("Fetching routine count");

                // Connect to database
                using (SqliteConnection connection = new(SqlitePathToURI(sqliteDatabasePath)))
                {
                    // Open connection
                    connection.Open();

                    // Fetch imports
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = $"SELECT * FROM {Routines.TABLE_NAME} "
                            + $"WHERE type = '{(int)RoutineType.Import}';";
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            bool read = false;
                            while (reader.Read())
                            {
                                if (read)
                                {
                                    throw new Exception("More than one import routine encountered");
                                }
                                read = true;
                                Routines routine = Routines.FromReader(reader);
                                importString = routine.code;
                            }
                        }
                    }

                    // Fetch row count
                    long routineCount = 0;
                    string routineWhereClause = $"WHERE code IS NOT NULL "
                        + $"AND code != '' "
                        + $"AND type != '{RoutineType.Import}'";
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = $"SELECT COUNT(*) as count "
                            + $"FROM {Routines.TABLE_NAME} {routineWhereClause};";
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) routineCount = reader.GetInt64(0);
                        }
                    }

                    // Fetch all routines
                    routinePath = Path.Combine(
                        routineOutputDirectory, $"{EditorConstants.k_RoutineInitializerClass}.cs");
                    using (StreamWriter writer = new StreamWriter(routinePath))
                    {
                        writer.NewLine = "\n";
                        WriteLine(writer, 0, $"// {EditorConstants.k_GeneratedCodeWarning}");
                        WriteLine(writer, 0, importString);
                        WriteLine(writer, 0, "");
                        WriteLine(writer, 0, $"namespace {RuntimeConstants.k_AppName}");
                        WriteLine(writer, 0, "{");
                        WriteLine(writer, 1,
                            $"public static class {EditorConstants.k_RoutineInitializerClass}");
                        WriteLine(writer, 1, "{");
                        WriteLine(writer, 2, "[UnityEngine.RuntimeInitializeOnLoadMethod("
                            + "UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen)]");
                        WriteLine(writer, 2, $"private static void Initialize()");
                        WriteLine(writer, 2, "{");
                        // +1 for noop
                        WriteLine(writer, 3, $"RoutineDirectory.Directory "
                            + $"= new System.Action<ConversationContext>[{routineCount + 1}];");

                        // Write Routines
                        uint currentIndex = 0;
                        for (uint i = 0; i < routineCount; i += EditorConstants.k_SqlBatchSize)
                        {
                            Progress.Report(
                                progressId, (float)i / routineCount, "Transpiling routines");
                            uint limit = EditorConstants.k_SqlBatchSize;
                            uint offset = i;
                            string query = $"SELECT * FROM {Routines.TABLE_NAME} "
                                + $"{routineWhereClause} "
                                + $"ORDER BY id ASC LIMIT {limit} OFFSET {offset};";
                            using (SqliteCommand command = connection.CreateCommand())
                            {
                                uint j = 0;
                                command.CommandType = CommandType.Text;
                                command.CommandText = query;
                                using (SqliteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        // Grab row
                                        Routines routine = Routines.FromReader(reader);

                                        // Transpile code
                                        currentIndex = i + j;
                                        j++;
                                        switch (routine.type)
                                        {
                                            case (int)RoutineType.User:
                                            case (int)RoutineType.Default:
                                                WriteRoutine(
                                                    routine, writer, flagCache, currentIndex,
                                                    routineIdToIndex);
                                                break;
                                            case (int)RoutineType.Import:
                                                if (routine.id != 1)
                                                {
                                                    throw new Exception(
                                                        "Import routine id was not 0 as expected");
                                                }
                                                WriteRoutine(
                                                    new() { id = routine.id, code = "" }, writer,
                                                    flagCache, currentIndex, routineIdToIndex);
                                                break;
                                            default:
                                                throw new Exception(
                                                    $"Unknown routine type encountered: "
                                                        + routine.type);
                                        }
                                    }
                                }
                            }
                        }

                        WriteLine(writer, 2, "}"); // Static block
                        WriteLine(writer, 1, "}"); // Class
                        WriteLine(writer, 0, "}"); // Namespace
                    }
                }

                // Write flags
                WriteFlags(flagOutputDirectory, flagCache);

                // Report done
                Progress.Report(progressId, 1f, "Done");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                if (!string.IsNullOrEmpty(routinePath) && File.Exists(routinePath))
                {
                    File.Delete(routinePath);
                }
                transpilerResult.WasError = true;
            }
            finally
            {
                Progress.Remove(progressId);
            }

            // Return transpile result
            transpilerResult.MaxFlags = (uint)flagCache.Count;
            transpilerResult.RoutineIdToIndex = routineIdToIndex;
            return transpilerResult;
        }
        #endregion

        #region Helpers
        static void WriteRoutine(
            Routines routine, StreamWriter writer, HashSet<string> flagCache, uint methodIndex,
            Dictionary<uint, uint> routineIdToIndex)
        {
            routineIdToIndex.Add((uint)routine.id, methodIndex);
            WriteLine(writer, 3,
                $"RoutineDirectory.Directory[{methodIndex}] = (ConversationContext ctx) =>");
            WriteLine(writer, 3, "{");
            try
            {
                string generatedCode = TranspilingTreeWalker.Transpile(routine, flagCache);
                if (generatedCode.Length > 0)
                {
                    string[] lines = generatedCode.Split("\n");
                    for (int i = 0; i < lines.Length; i++)
                    {
                        WriteLine(writer, 4, lines[i]);
                    }
                }
            }
            catch (Exception e)
            {
                WriteLine(writer, 3, $"    /* Error in routine: {routine.id} */");
                Debug.LogException(e);
            }
            WriteLine(writer, 3, "};");
        }

        static void WriteFlags(string outputDirectory, HashSet<string> flagCache)
        {
            string path = Path.Combine(outputDirectory, $"{EditorConstants.k_RoutineFlagEnum}.cs");
            if (File.Exists(path)) File.Delete(path);
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.NewLine = "\n";
                WriteLine(writer, 0, $"// {EditorConstants.k_GeneratedCodeWarning}");
                WriteLine(writer, 0, "");
                WriteLine(writer, 0, $"namespace {RuntimeConstants.k_AppName}");
                WriteLine(writer, 0, "{");
                WriteLine(writer, 1, $"public enum {EditorConstants.k_RoutineFlagEnum}");
                WriteLine(writer, 1, "{");
                foreach (string flag in flagCache)
                {
                    WriteLine(writer, 2, flag + ",");
                }
                WriteLine(writer, 1, "}"); // enum
                WriteLine(writer, 0, "}"); // namespace
            }
        }
        #endregion
    }
}
