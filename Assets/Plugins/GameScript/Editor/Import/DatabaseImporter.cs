using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine;

namespace GameScript
{
    public static class DatabaseImporter
    {
        #region Constants
        private static readonly string k_DbCodeOutputDirectory = Path.Combine(
            Application.dataPath, "Plugins", "GameScript", "Editor", "Generated", "SQLite");
        private static readonly string k_FlagOutputDirectory
            = Path.Combine(Application.dataPath, "Plugins", "GameScript", "Runtime", "Generated");
        #endregion

        #region Variables
        public static bool IsImporting { get; private set; } = false;
        #endregion

        #region API
        public static string GetDatabaseVersion(string sqliteDatabasePath)
        {
            using (SqliteConnection connection = new(Database.SqlitePathToURI(sqliteDatabasePath)))
            {
                // Open connection
                connection.Open();

                // Fetch table names
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT version FROM {Version.TABLE_NAME};";
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) return reader.GetString(0);
                    }
                }
            }
            return null;
        }

        public static void ImportDatabase(string sqliteDatabasePath, string routineOutputDirectory,
            string conversationOutputDirectory)
                => ImportDatabase(sqliteDatabasePath, routineOutputDirectory,
                    conversationOutputDirectory, k_DbCodeOutputDirectory, k_FlagOutputDirectory);
        public static async void ImportDatabase(
            string sqliteDatabasePath, string routineOutputDirectory,
            string conversationOutputDirectory, string dbCodeDirectory, string flagOutputDirectory)
        {
            try
            {
                IsImporting = true;
                TranspilerResult result = new();
                await Task.Run(() =>
                {
                    DatabaseCodeGenerator.GenerateDatabaseCode(sqliteDatabasePath, dbCodeDirectory);
                    result = Transpiler.Transpile(
                        sqliteDatabasePath, routineOutputDirectory, flagOutputDirectory);
                    ConversationDataGenerator.GenerateConversationData(sqliteDatabasePath,
                        conversationOutputDirectory, result.RoutineIdToIndex, result.NoopRoutineId);
                });

                // Update Settings
                Settings.Instance.MaxFlags = result.MaxFlags;

                // Refresh Database
                // AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                IsImporting = false;
            }
        }
        #endregion
    }
}