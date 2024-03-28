using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine;

namespace GameScript
{
    static class DatabaseImporter
    {
        #region Constants
        private const string k_ImportInProgress = "IMPORT_IN_PROGRESS";
        private const string k_SQLiteDatabasePathKey = "SQLITE_DATABASE_PATH";
        private const string k_FlagOutputDirectoryKey = "FLAG_OUTPUT_DIRECTORY";
        private const string k_RoutineOutputDirectoryKey = "ROUTINE_OUTPUT_DIRECTORY";
        private const string k_ConversationOutputDirectoryKey = "CONVERSATION_OUTPUT_DIRECTORY";
        private static readonly string k_DbCodeOutputDirectory = Path.Combine(
            Application.dataPath,
            "Plugins",
            "GameScript",
            "Editor",
            "Generated",
            "SQLite"
        );
        private static readonly string k_FlagOutputDirectory = Path.Combine(
            Application.dataPath,
            "Plugins",
            "GameScript",
            "Runtime",
            "Generated"
        );
        #endregion

        #region Variables
        public static bool IsImporting { get; private set; } = false;
        #endregion

        #region API
        public static string GetDatabaseVersion(string sqliteDatabasePath)
        {
#if GAMESCRIPT_CODE_GENERATED
            using (SqliteConnection connection = new(DbHelper.SqlitePathToURI(sqliteDatabasePath)))
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
                        while (reader.Read())
                            return reader.GetString(0);
                    }
                }
            }
#endif
            return null;
        }

        public static void ImportDatabase(
            string sqliteDatabasePath,
            string routineOutputDirectory,
            string conversationOutputDirectory
        ) =>
            ImportDatabase(
                sqliteDatabasePath,
                routineOutputDirectory,
                conversationOutputDirectory,
                k_DbCodeOutputDirectory,
                k_FlagOutputDirectory
            );

        public static async void ImportDatabase(
            string sqliteDatabasePath,
            string routineOutputDirectory,
            string conversationOutputDirectory,
            string dbCodeDirectory,
            string flagOutputDirectory
        )
        {
            try
            {
                EditorPrefs.SetBool(k_ImportInProgress, true);
                EditorPrefs.SetString(k_SQLiteDatabasePathKey, sqliteDatabasePath);
                EditorPrefs.SetString(k_FlagOutputDirectoryKey, flagOutputDirectory);
                EditorPrefs.SetString(k_RoutineOutputDirectoryKey, routineOutputDirectory);
                EditorPrefs.SetString(
                    k_ConversationOutputDirectoryKey,
                    conversationOutputDirectory
                );

                DbCodeGeneratorResult codeGenResult = default;

                await Task.Run(() =>
                {
                    codeGenResult = DatabaseCodeGenerator.GenerateDatabaseCode(
                        sqliteDatabasePath,
                        dbCodeDirectory
                    );
                });
                if (codeGenResult.WasError)
                    throw new Exception("Failed to generate objects from database schema");

                // Refresh Database
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                IsImporting = false;
                EditorPrefs.SetBool(k_ImportInProgress, false);
            }
        }
        #endregion

        #region On Scripts Recompile
        [UnityEditor.Callbacks.DidReloadScripts]
#if GAMESCRIPT_CODE_GENERATED
        private static async void OnScriptsReloaded()
#else
        private static void OnScriptsReloaded()
#endif
        {
#if GAMESCRIPT_CODE_GENERATED
            if (EditorPrefs.GetBool(k_ImportInProgress))
            {
                string sqliteDatabasePath = EditorPrefs.GetString(k_SQLiteDatabasePathKey);
                string flagOutputDirectory = EditorPrefs.GetString(k_FlagOutputDirectoryKey);
                string routineOutputDirectory = EditorPrefs.GetString(k_RoutineOutputDirectoryKey);
                string conversationOutputDirectory = EditorPrefs.GetString(
                    k_ConversationOutputDirectoryKey
                );

                TranspilerResult transpilerResult = default;
                ConversationDataGeneratorResult conversationResult = default;
                ReferenceGeneratorResult assetResult = default;
                try
                {
                    await Task.Run(() =>
                    {
                        transpilerResult = Transpiler.Transpile(
                            sqliteDatabasePath,
                            routineOutputDirectory,
                            flagOutputDirectory
                        );
                        if (transpilerResult.WasError)
                            return;
                        conversationResult = ConversationDataGenerator.GenerateConversationData(
                            sqliteDatabasePath,
                            conversationOutputDirectory,
                            transpilerResult.RoutineIdToIndex
                        );
                    });

                    // Check for errors
                    if (transpilerResult.WasError || conversationResult.WasError)
                        return;

                    // Create asset references (must be main thread)
                    assetResult = ReferenceGenerator.GenerateAssetReferences(
                        sqliteDatabasePath,
                        routineOutputDirectory
                    );
                    if (assetResult.WasError)
                        return;

                    // Update Settings
                    Settings.Instance.MaxFlags = transpilerResult.MaxFlags;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    EditorPrefs.SetBool(k_ImportInProgress, false);
                    IsImporting = false;
                }
            }
#endif
        }
        #endregion
    }
}
