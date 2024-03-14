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
        public static ConversationDataGeneratorResult GenerateConversationData(
            string dbPath, string conversationDataPath, Dictionary<uint, uint> routineIdToIndex)
        {
            int progressId = 0;
            ConversationDataGeneratorResult result = new();
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
                    progressId, dbPath, routineIdToIndex);

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
                result.WasError = true;
            }
            finally
            {
                Progress.Remove(progressId);
            }
            return result;
        }

        static Disk CreateSerializedData(
            int progressId, string dbPath, Dictionary<uint, uint> routineIdToIndex)
        {
            Disk disk = new();
            using (SqliteConnection connection = new(Database.SqlitePathToURI(dbPath)))
            {
                connection.Open();
                Dictionary<uint, DiskLocalization> idToLocalization = new();
                Progress.Report(progressId, 0.1f, "Gathering localizations");
                disk.localizations = SerializeLocalizations(connection, idToLocalization);
                Progress.Report(progressId, 0.3f, "Gathering locales");
                disk.locales = SerializeLocales(connection, idToLocalization);
                Progress.Report(progressId, 0.4f, "Gathering actors");
                disk.actors = SerializeActors(connection, idToLocalization);
                Progress.Report(progressId, 0.5f, "Gathering conversations");
                disk.conversations = SerializeConversations(
                    connection, idToLocalization, routineIdToIndex);
            }
            return disk;
        }

        /**We only care about non-system created localizations*/
        static DiskLocalization[] SerializeLocalizations(
            SqliteConnection connection, Dictionary<uint, DiskLocalization> idToLocalization)
        {
            DiskLocalization[] localizations = null;

            // Grab locale fields
            List<FieldInfo> localizationFields = new();
            FieldInfo[] fieldInfos = typeof(Localizations)
                .GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo info = fieldInfos[i];
                if (info.Name.StartsWith("locale_")) localizationFields.Add(info);
            }

            ReadTable(connection, Localizations.TABLE_NAME,
            (uint count) =>
            {
                localizations = new DiskLocalization[count];
            },
            (uint index, SqliteDataReader reader) =>
            {
                Localizations localization = Localizations.FromReader(reader);

                // Grab list of localized string in order of locale id
                string[] localizationStrings = new string[localizationFields.Count];
                for (int j = 0; j < localizationFields.Count; j++)
                {
                    FieldInfo info = localizationFields[j];
                    localizationStrings[j] = (string)info.GetValue(localization);
                }

                // Create disk localization
                DiskLocalization diskLocalization = new()
                {
                    id = (uint)localization.id,
                    localizations = localizationStrings,
                };

                // Add disk localization to lookup table
                idToLocalization.Add(diskLocalization.id, diskLocalization);

                // Add localization to list (if this is non-system created)
                if (!localization.isSystemCreated)
                {
                    localizations[index] = diskLocalization;
                }
            });
            return localizations;
        }

        static DiskLocale[] SerializeLocales(
            SqliteConnection connection, Dictionary<uint, DiskLocalization> idToLocalization)
        {
            DiskLocale[] locales = null;
            ReadTable(connection, Locales.TABLE_NAME,
            (uint count) =>
            {
                locales = new DiskLocale[count];
            },
            (uint index, SqliteDataReader reader) =>
            {
                Locales locale = Locales.FromReader(reader);
                locales[index] = new()
                {
                    id = (uint)locale.id,
                    index = index,
                    name = locale.name,
                    localizedName = idToLocalization[(uint)locale.localizedName],
                };
            });
            return locales;
        }

        static DiskActor[] SerializeActors(
            SqliteConnection connection, Dictionary<uint, DiskLocalization> idToLocalization)
        {
            DiskActor[] actors = null;
            ReadTable(connection, Actors.TABLE_NAME,
            (uint count) =>
            {
                actors = new DiskActor[count];
            },
            (uint index, SqliteDataReader reader) =>
            {
                Actors actor = Actors.FromReader(reader);
                actors[index] = new()
                {
                    id = (uint)actor.id,
                    name = actor.name,
                    localizedName = idToLocalization[(uint)actor.localizedName],
                };
            });
            return actors;
        }

        static DiskConversation[] SerializeConversations(SqliteConnection connection,
            Dictionary<uint, DiskLocalization> idToLocalization,
            Dictionary<uint, uint> routineIdToIndex)
        {
            DiskConversation[] conversations = null;
            ReadTable(connection, Conversations.TABLE_NAME,
            (uint count) =>
            {
                conversations = new DiskConversation[count];
            },
            (uint index, SqliteDataReader reader) =>
            {
                Conversations conversation = Conversations.FromReader(reader);
                conversations[index] = new()
                {
                    id = (uint)conversation.id,
                    name = conversation.name,
                    nodes = FetchNodesForConversation(
                        connection, (uint)conversation.id, idToLocalization, routineIdToIndex),
                    edges = FetchEdgesForConversation(connection, (uint)conversation.id),
                };
            });

            return conversations;
        }

        static DiskNode[] FetchNodesForConversation(SqliteConnection connection,
            uint conversationId, Dictionary<uint, DiskLocalization> idToLocalization,
            Dictionary<uint, uint> routineIdToIndex)
        {
            return FetchConversationChildObjects(
                connection, Nodes.TABLE_NAME, conversationId, (SqliteDataReader reader) =>
            {
                Nodes node = Nodes.FromReader(reader);

                // Note:
                // Root nodes don't have localizations or code.
                bool isRoot = node.type == "root";
                DiskLocalization uiResponseText = null;
                DiskLocalization voiceText = null;
                if (!isRoot)
                {
                    uiResponseText = idToLocalization[(uint)node.uiResponseText];
                    voiceText = idToLocalization[(uint)node.voiceText];
                }
                // Note: 
                // If these don't exist in the map, it means they were noop routines. Moreover,
                // if these were null (for root nodes), they'd default to 0 and not exist in the
                // map. For either of these cases, we set the value to 0 because that's where the
                // noop routine lives. It's actually the "import" routine since that must
                // always be the first routine the DB by convention.
                uint condition = routineIdToIndex.ContainsKey((uint)node.condition)
                    ? routineIdToIndex[(uint)node.condition]
                    : 0;
                uint code = routineIdToIndex.ContainsKey((uint)node.code)
                    ? routineIdToIndex[(uint)node.code]
                    : 0;
                return new DiskNode()
                {
                    id = (uint)node.id,
                    actor = (uint)node.actor,
                    uiResponseText = uiResponseText,
                    voiceText = voiceText,
                    condition = condition,
                    code = code,
                    isPreventResponse = node.isPreventResponse,
                    isRoot = isRoot,
                };
            });
        }

        static DiskEdge[] FetchEdgesForConversation(
            SqliteConnection connection, uint conversationId)
        {
            return FetchConversationChildObjects(
                connection, Edges.TABLE_NAME, conversationId, (SqliteDataReader reader) =>
            {
                Edges edge = Edges.FromReader(reader);
                return new DiskEdge()
                {
                    id = (uint)edge.id,
                    source = (uint)edge.source,
                    target = (uint)edge.target,
                    priority = edge.priority,
                };
            });
        }

        static T[] FetchConversationChildObjects<T>(
            SqliteConnection connection, string tableName, uint conversationId,
            Func<SqliteDataReader, T> childCreator)
        {
            long count = 0;
            string whereClause = $"WHERE parent = {conversationId}";
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText
                    = $"SELECT COUNT(*) as count FROM {tableName} {whereClause};";
                using (SqliteDataReader nodeReader = command.ExecuteReader())
                {
                    while (nodeReader.Read()) count = nodeReader.GetInt64(0);
                }
            }

            string nodeQuery = $"SELECT * FROM {tableName} {whereClause};";
            T[] objs = new T[count];
            using (SqliteCommand command = connection.CreateCommand())
            {
                uint j = 0;
                command.CommandType = CommandType.Text;
                command.CommandText = nodeQuery;
                using (SqliteDataReader nodeReader = command.ExecuteReader())
                {
                    while (nodeReader.Read()) objs[j++] = childCreator(nodeReader);
                }
            }
            return objs;
        }

        static void ReadTable(
            SqliteConnection connection, string tableName, Action<uint> onCount,
            Action<uint, SqliteDataReader> onRow)
        {
            // Fetch row count
            uint count = 0;
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT COUNT(*) as count FROM {tableName};";
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) count = (uint)reader.GetInt64(0);
                }
            }
            onCount(count);

            // Fetch all rows 
            for (uint i = 0; i < count; i += EditorConstants.k_SqlBatchSize)
            {
                uint limit = EditorConstants.k_SqlBatchSize;
                uint offset = i;
                string query = $"SELECT * FROM {tableName} "
                    + $"ORDER BY id ASC LIMIT {limit} OFFSET {offset};";
                uint j = 0;
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) onRow(i + j++, reader);
                    }
                }
            }
        }
    }
}
