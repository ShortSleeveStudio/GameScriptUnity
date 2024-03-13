using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEditor;
using System.IO;
using static GameScript.StringWriter;
using static GameScript.Database;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace GameScript
{
    public class Transpiler
    {

        public static async void Transpile(
            string sqliteDatabasePath, string routineOutputDirectory, string flagOutputDirectory)
        {
            await Task.Run(()
                => TranspileAsync(sqliteDatabasePath, routineOutputDirectory, flagOutputDirectory));
            AssetDatabase.Refresh();
        }

        /**
         * This will build an array of Action's that will be looked up by index. The final index
         * contains the "noop" Action used for all empty/null routines.
         */
        static void TranspileAsync(
            string sqliteDatabasePath, string routineOutputDirectory, string flagOutputDirectory)
        {
            // Create flag cache
            HashSet<string> flagCache = new();
            string importString = "";

            int progressId = 0;
            try
            {
                // Start progress tracking
                progressId = Progress.Start("Fetching routine count");

                // Connect to database
                using (SqliteConnection connection = new(SQLitePathToURI(sqliteDatabasePath)))
                {
                    // Open connection
                    connection.Open();

                    // Fetch imports
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        // Get routine count
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
                        // Get routine count
                        command.CommandType = CommandType.Text;
                        command.CommandText = $"SELECT COUNT(*) as count "
                            + $"FROM {Routines.TABLE_NAME} {routineWhereClause};";
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) routineCount = reader.GetInt64(0);
                        }
                    }

                    // Fetch all routines
                    string path = Path.Combine(
                        routineOutputDirectory, $"{EditorConstants.ROUTINE_INITIALIZER_CLASS}.cs");
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        writer.NewLine = "\n";
                        WriteLine(writer, 0, $"// {EditorConstants.GENERATED_CODE_WARNING}");
                        WriteLine(writer, 0, importString);
                        WriteLine(writer, 0, "");
                        WriteLine(writer, 0, $"namespace {RuntimeConstants.APP_NAME}");
                        WriteLine(writer, 0, "{");
                        WriteLine(writer, 1,
                            $"public static class {EditorConstants.ROUTINE_INITIALIZER_CLASS}");
                        WriteLine(writer, 1, "{");
                        WriteLine(writer, 2, "[UnityEngine.RuntimeInitializeOnLoadMethod("
                            + "UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen)]");
                        WriteLine(writer, 2, $"private static void Initialize()");
                        WriteLine(writer, 2, "{");
                        // +1 for noop
                        WriteLine(writer, 3, $"RoutineDirectory.Directory "
                            + $"= new System.Action<ConversationContext>[{routineCount + 1}];");

                        // Write Routines
                        int currentIndex = 0;
                        for (int i = 0; i < routineCount; i += EditorConstants.SQL_BATCH_SIZE)
                        {
                            Progress.Report(
                                progressId, (float)i / routineCount, "Transpiling routines");
                            int limit = EditorConstants.SQL_BATCH_SIZE;
                            int offset = i;
                            string query = $"SELECT * FROM {Routines.TABLE_NAME} "
                                + $"{routineWhereClause} "
                                + $"ORDER BY id ASC LIMIT {limit} OFFSET {offset};";
                            using (SqliteCommand command = connection.CreateCommand())
                            {
                                // Get routine count
                                int j = 0;
                                command.CommandType = CommandType.Text;
                                command.CommandText = query;
                                using (SqliteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        // Grab routine
                                        Routines routine = Routines.FromReader(reader);

                                        // Transpile code
                                        currentIndex = i + j;
                                        j++;
                                        switch (routine.type)
                                        {
                                            case (int)RoutineType.User:
                                            case (int)RoutineType.Default:
                                                WriteRoutine(
                                                    routine, writer, flagCache, currentIndex);
                                                break;
                                            case (int)RoutineType.Import:
                                                WriteRoutine(
                                                    new() { code = "" }, writer, flagCache,
                                                    currentIndex);
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

                        // Write the noop routine
                        WriteRoutine(new() { code = "" }, writer, flagCache, ++currentIndex);

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
            }
            finally
            {
                Progress.Remove(progressId);
            }
        }

        static void WriteRoutine(
            Routines routine, StreamWriter writer, HashSet<string> flagCache,
            int methodIndex)
        {
            WriteLine(writer, 3,
                $"RoutineDirectory.Directory[{methodIndex}] = (ConversationContext ctx) =>");
            WriteLine(writer, 3, "{");

            // Create parser
            TranspileErrorListener errorListener = new();
            ICharStream stream = CharStreams.fromString(routine.code.Trim());
            CSharpRoutineLexer lexer = new CSharpRoutineLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CSharpRoutineParser parser = new(tokens)
            {
                BuildParseTree = true,
            };
            lexer.RemoveErrorListeners();
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);
            try
            {
                IParseTree tree = routine.isCondition ? parser.expression() : parser.routine();
                string generatedCode = TranspilingTreeWalker.Transpile(tree, flagCache, routine);
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
                Debug.LogError($"Transpilation error in routine {routine.id} at "
                    + $"line: {errorListener.ErrorLine} "
                    + $"column: {errorListener.ErrorColumn} "
                    + $"message: {errorListener.ErrorMessage}");
                Debug.LogException(e);
            }
            WriteLine(writer, 3, "};");
        }

        static void WriteFlags(string outputDirectory, HashSet<string> flagCache)
        {
            string path = Path.Combine(outputDirectory, $"{EditorConstants.ROUTINE_FLAG_ENUM}.cs");
            if (File.Exists(path)) File.Delete(path);
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.NewLine = "\n";
                WriteLine(writer, 0, $"// {EditorConstants.GENERATED_CODE_WARNING}");
                WriteLine(writer, 0, "");
                WriteLine(writer, 0, $"namespace {RuntimeConstants.APP_NAME}");
                WriteLine(writer, 0, "{");
                WriteLine(writer, 1, $"public enum {EditorConstants.ROUTINE_FLAG_ENUM}");
                WriteLine(writer, 1, "{");
                foreach (string flag in flagCache)
                {
                    WriteLine(writer, 2, flag + ",");
                }
                WriteLine(writer, 1, "}"); // enum
                WriteLine(writer, 0, "}"); // namespace
            }
        }
    }
}
