using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine;

namespace GameScript
{
    public static class ConversationDataGenerator
    {
        public static void GenerateConversationData(
            string dbPath, string conversationDataPath, Dictionary<uint, uint> routineIdToIndex,
            uint noopRoutineId)
        {
            int progressId = 0;
            try
            {
                // Start progress tracking
                progressId = Progress.Start("Serializing conversation graphs");

                // Recreate directory 
                if (Directory.Exists(conversationDataPath))
                {
                    Directory.Delete(conversationDataPath, true);
                }
                Directory.CreateDirectory(conversationDataPath);
                string path = Path.Combine(
                    conversationDataPath, RuntimeConstants.k_ConversationDataFilename);

                // Create the data
                Disk toSerialize = CreateSerializedData(
                    progressId, dbPath, routineIdToIndex, noopRoutineId);
                Debug.Log(JsonUtility.ToJson(toSerialize));

                // Write to disk
                Progress.Report(progressId, 0.7f, "Serializing data");
                BinaryFormatter serializer = new();
                using (FileStream fs = new(path, FileMode.Create))
                {
                    serializer.Serialize(fs, toSerialize);
                }
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

        static Disk CreateSerializedData(
            int progressId, string dbPath, Dictionary<uint, uint> routineIdToIndex,
            uint noopRoutineId)
        {
            Progress.Report(progressId, 0.1f, "Gathering localizations");
            List<DiskLocalization> localizations = SerializeLocalizations(dbPath);
            Progress.Report(progressId, 0.3f, "Gathering locales");
            List<DiskLocale> locales = SerializeLocales(dbPath);
            Progress.Report(progressId, 0.4f, "Gathering actors");
            List<DiskActor> actors = SerializeActors(dbPath);
            Progress.Report(progressId, 0.5f, "Gathering conversations");
            List<DiskConversation> conversations = SerializeConversations(dbPath);
            return new Disk()
            {
                localizations = localizations,
                locales = locales,
                actors = actors,
                conversations = conversations,
            };
        }

        /**We only care about non-system created localizations*/
        static List<DiskLocalization> SerializeLocalizations(string dbPath)
        {
            List<DiskLocalization> localizations = new();

            // Grab locale fields
            List<FieldInfo> localizationFields = new();
            FieldInfo[] fieldInfos = typeof(Localizations)
                .GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo info = fieldInfos[i];
                if (info.Name.StartsWith("locale_")) localizationFields.Add(info);
            }

            using (SqliteConnection connection = new(Database.SqlitePathToURI(dbPath)))
            {
                // Open connection
                connection.Open();

                // Fetch row count
                long localizationCount = 0;
                string whereClause = "WHERE isSystemCreated = false";
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT COUNT(*) as count "
                        + $"FROM {Localizations.TABLE_NAME} {whereClause};";
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) localizationCount = reader.GetInt64(0);
                    }
                }

                // Fetch all localizations
                for (uint i = 0; i < localizationCount; i += EditorConstants.k_SqlBatchSize)
                {
                    uint limit = EditorConstants.k_SqlBatchSize;
                    uint offset = i;
                    string query = $"SELECT * FROM {Localizations.TABLE_NAME} "
                        + $"{whereClause} "
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
                                // Grab localization
                                Localizations localization = Localizations.FromReader(reader);

                                // Grab list of localized string in order of locale id
                                string[] localizationStrings = new string[localizationFields.Count];
                                for (int j = 0; j < localizationFields.Count; j++)
                                {
                                    FieldInfo info = localizationFields[j];
                                    localizationStrings[j] = (string)info.GetValue(localization);
                                }

                                // Add localization to list
                                localizations.Add(new()
                                {
                                    id = (uint)localization.id,
                                    localizations = localizationStrings,
                                });
                            }
                        }
                    }
                }
            }
            return localizations;
        }

        static List<DiskLocale> SerializeLocales(string dbPath)
        {
            List<DiskLocale> locales = new();
            return locales;
        }

        static List<DiskActor> SerializeActors(string dbPath)
        {
            List<DiskActor> actors = new();
            return actors;
        }

        static List<DiskConversation> SerializeConversations(string dbPath)
        {
            List<DiskConversation> conversations = new();
            return conversations;
        }
    }
}
