using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEditor;

namespace GameScript
{
    public class Transpiler
    {
        public static async void Transpile(string sqliteDatabasePath, string outputDirectory)
        {
            await Task.Run(() => TranspileAsync(sqliteDatabasePath, outputDirectory));
        }

        static void TranspileAsync(string sqliteDatabasePath, string outputDirectory)
        {
            // Create flag cache
            HashSet<string> flagCache = new();

            int progressId = 0;
            try
            {
                // Start progress tracking
                progressId = Progress.Start("Fetching routine count");

                // Connect to database
                using (SqliteConnection connection = new(Database.SQLitePathToURI(sqliteDatabasePath)))
                {
                    // Open connection
                    connection.Open();

                    // Fetch row count
                    long routineCount = 0;
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        // Get routine count
                        command.CommandType = CommandType.Text;
                        command.CommandText = $"SELECT COUNT(*) as count FROM {Routines.TABLE_NAME}";
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) routineCount = reader.GetInt64(0);
                        }
                    }

                    // Fetch all routines
                    for (int i = 0; i < routineCount; i += Constants.SQL_BATCH_SIZE)
                    {
                        Progress.Report(progressId, (float)i / routineCount, "Transpiling routines");
                        int limit = Constants.SQL_BATCH_SIZE;
                        int offset = i;
                        string query = $"SELECT * FROM {Routines.TABLE_NAME} "
                            + $"ORDER BY id ASC LIMIT {limit} OFFSET {offset};";
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            // Get routine count
                            command.CommandType = CommandType.Text;
                            command.CommandText = query;
                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Grab routine
                                    Routines routine = Routines.FromReader(reader);

                                    // Skip empty routines
                                    if (routine.code == null || routine.code.Trim().Length == 0)
                                    {
                                        Debug.Log("Skipping empty code");
                                        continue;
                                    }

                                    // Transpile code 
                                    switch (routine.type)
                                    {
                                        case (int)RoutineType.User:
                                            Debug.Log("USER: ");
                                            Debug.Log(routine.code);
                                            break;
                                        case (int)RoutineType.Default:
                                            Debug.Log("DEFAULT: ");
                                            Debug.Log(routine.code);
                                            break;
                                        case (int)RoutineType.Import:
                                            Debug.Log("IMPORT: ");
                                            Debug.Log(routine.code);
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
    }
}
