using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEditor;
using System.IO;
using static Database;

namespace GameScript
{
    public class Transpiler
    {

        public static async void Transpile(string sqliteDatabasePath, string outputDirectory)
        {
            await Task.Run(() => TranspileAsync(sqliteDatabasePath, outputDirectory));
            AssetDatabase.Refresh();
        }

        /**
         * This will build an array of Action's that will be looked up by index. The final index
         * contains the "noop" Action used for all empty/null routines.
         */
        static void TranspileAsync(string sqliteDatabasePath, string outputDirectory)
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
                                Debug.Log("IMPORT IMPORT IMPORT" + importString);
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
                    using (StreamWriter writer = new StreamWriter(
                        Path.Combine(outputDirectory, "RoutineDirectory.cs")))
                    {
                        WriteLine(writer, 0, $"// {Constants.GENERATED_CODE_WARNING}");
                        WriteLine(writer, 0, importString);
                        WriteLine(writer, 0, "");
                        WriteLine(writer, 0, $"namespace {Constants.APP_NAME}");
                        WriteLine(writer, 0, "{");
                        WriteLine(writer, 1,
                            $"public static class {Constants.ROUTINE_DIRECTORY_CLASS}");
                        WriteLine(writer, 1, "{");
                        WriteLine(writer, 2, $"public static System.Action[] Directory "
                            + $"= new System.Action[{routineCount + 1}];"); // Add one for noop
                        WriteLine(writer, 2, $"static {Constants.ROUTINE_DIRECTORY_CLASS}()");
                        WriteLine(writer, 2, "{");

                        // Write Routines
                        int currentIndex = 0;
                        for (int i = 0; i < routineCount; i += Constants.SQL_BATCH_SIZE)
                        {
                            Progress.Report(
                                progressId, (float)i / routineCount, "Transpiling routines");
                            int limit = Constants.SQL_BATCH_SIZE;
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

                    // Report done
                    Progress.Report(progressId, 1f, "Done");
                }
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
            WriteLine(writer, 3, $"Directory[{methodIndex}] = () =>");
            WriteLine(writer, 3, "{");
            WriteLine(writer, 4, "/* Code Goes Here");
            WriteLine(writer, 4, routine.code);
            WriteLine(writer, 4, "*/");
            WriteLine(writer, 3, "};");
        }
    }
}
