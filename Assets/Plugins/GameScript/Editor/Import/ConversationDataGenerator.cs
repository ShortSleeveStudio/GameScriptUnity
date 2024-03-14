using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
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
                GameData toSerialize = CreateSerializedData(
                    progressId, dbPath, routineIdToIndex);

                // Write to disk
                Progress.Report(progressId, 0.7f, "Serializing data");
                BinaryFormatter serializer = new();
                using (FileStream fs = new(path, FileMode.Create))
                {
                    using (GZipStream zipStream = new(fs, CompressionMode.Compress))
                    {
                        serializer.Serialize(zipStream, toSerialize);
                    }
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

        static GameData CreateSerializedData(
            int progressId, string dbPath, Dictionary<uint, uint> routineIdToIndex)
        {
            GameData disk = new();
            using (SqliteConnection connection = new(Database.SqlitePathToURI(dbPath)))
            {
                connection.Open();
                Dictionary<uint, Localization> idToLocalization = new();
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
        static Localization[] SerializeLocalizations(
            SqliteConnection connection, Dictionary<uint, Localization> idToLocalization)
        {
            Localization[] localizations = null;

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
                localizations = new Localization[count];
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
                Localization diskLocalization = new()
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

        static Locale[] SerializeLocales(
            SqliteConnection connection, Dictionary<uint, Localization> idToLocalization)
        {
            Locale[] locales = null;
            ReadTable(connection, Locales.TABLE_NAME,
            (uint count) =>
            {
                locales = new Locale[count];
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

        static Actor[] SerializeActors(
            SqliteConnection connection, Dictionary<uint, Localization> idToLocalization)
        {
            Actor[] actors = null;
            ReadTable(connection, Actors.TABLE_NAME,
            (uint count) =>
            {
                actors = new Actor[count];
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

        static Conversation[] SerializeConversations(SqliteConnection connection,
            Dictionary<uint, Localization> idToLocalization,
            Dictionary<uint, uint> routineIdToIndex)
        {
            // Gather all conversation data
            Conversation[] conversations = null;
            Dictionary<uint, Edge> nodeIdToEdgeMissingTarget = new();
            Dictionary<uint, Node> idToNode = new(); // All nodes in game
            ReadTable(connection, Conversations.TABLE_NAME,
            (uint count) =>
            {
                conversations = new Conversation[count];
            },
            (uint index, SqliteDataReader reader) =>
            {
                Conversations conversation = Conversations.FromReader(reader);
                Node root;
                conversations[index] = new()
                {
                    id = (uint)conversation.id,
                    name = conversation.name,
                    nodes = FetchNodesForConversation(
                        connection, (uint)conversation.id, idToLocalization, routineIdToIndex,
                        nodeIdToEdgeMissingTarget, idToNode, out root),
                    rootNode = root,
                };
            },
            "WHERE isDeleted = false");

            // Handle all edges that link outside of their conversations
            foreach (KeyValuePair<uint, Edge> entry in nodeIdToEdgeMissingTarget)
            {
                entry.Value.target = idToNode[entry.Key];
            }

            return conversations;
        }

        static Node[] FetchNodesForConversation(SqliteConnection connection,
            uint conversationId, Dictionary<uint, Localization> idToLocalization,
            Dictionary<uint, uint> routineIdToIndex,
            Dictionary<uint, Edge> nodeIdToEdgeMissingTarget,
            Dictionary<uint, Node> idToNode, out Node rootNode)
        {
            // Gather all nodes, populated without edges
            Dictionary<uint, List<Edge>> nodeIdToOutgoingEdges = new();
            Node root = null;
            Node[] nodes = FetchConversationChildObjects(
                connection, Nodes.TABLE_NAME, conversationId, (SqliteDataReader reader) =>
            {
                Nodes node = Nodes.FromReader(reader);

                // Note: 
                // If these don't exist in the map, it means they were noop routines. Moreover,
                // if these were null (for root nodes), they'd default to 0 and not exist in the
                // map. For either of these cases, we set the value to 0 because that's where the
                // noop routine lives. It's actually the "import" routine since that must
                // always be the first routine the DB by convention.
                uint condition = routineIdToIndex.ContainsKey((uint)node.condition)
                    ? routineIdToIndex[(uint)node.condition]
                    : 0;
                // Handle default routines
                if (node.codeOverride != 0) node.code = node.codeOverride;
                uint code = routineIdToIndex.ContainsKey((uint)node.code)
                      ? routineIdToIndex[(uint)node.code]
                      : 0;
                Node diskNode = new Node()
                {
                    id = (uint)node.id,
                    actor = (uint)node.actor,

                    condition = condition,
                    code = code,
                    isPreventResponse = node.isPreventResponse,
                };

                // Note: Root nodes don't have localizations or code.
                if (node.type == "root")
                {
                    diskNode.uiResponseText = null;
                    diskNode.voiceText = null;
                    root = diskNode;
                }
                else
                {
                    diskNode.uiResponseText = idToLocalization[(uint)node.uiResponseText];
                    diskNode.voiceText = idToLocalization[(uint)node.voiceText];
                }

                // Add to node lookup table
                idToNode.Add(diskNode.id, diskNode);
                return diskNode;
            });
            rootNode = root;

            // Gather edges and populate nodes
            // Note: because we may eventually wish to link from one conversation to another
            //       we'll maintain a map of edges that are missing targets.
            Edge[] edges = FetchConversationChildObjects(connection, Edges.TABLE_NAME,
                conversationId, (SqliteDataReader reader) =>
            {
                Edges edge = Edges.FromReader(reader);

                uint sourceId = (uint)edge.source;
                uint targetId = (uint)edge.target;
                Edge diskEdge = new Edge()
                {
                    id = (uint)edge.id,
                    source = idToNode[sourceId],
                    priority = edge.priority,
                };

                // Set target node
                Node targetNode;
                idToNode.TryGetValue(targetId, out targetNode);
                if (targetNode == null) nodeIdToEdgeMissingTarget.Add(targetId, diskEdge);
                else diskEdge.target = targetNode;

                // Add to outgoing edge list
                AddToEdgeList(nodeIdToOutgoingEdges, sourceId, diskEdge);

                return diskEdge;
            });

            // Populate node's edge field
            for (int i = 0; i < nodes.Length; i++)
            {
                Node node = nodes[i];
                List<Edge> outgoingEdges;
                nodeIdToOutgoingEdges.TryGetValue(node.id, out outgoingEdges);
                if (outgoingEdges == null) node.outgoingEdges = new Edge[0];
                else node.outgoingEdges = nodeIdToOutgoingEdges[node.id].ToArray();
            }
            return nodes;
        }

        static void AddToEdgeList(Dictionary<uint, List<Edge>> map, uint nodeId, Edge edge)
        {
            List<Edge> edgeList;
            map.TryGetValue(nodeId, out edgeList);
            if (edgeList == null)
            {
                edgeList = new();
                map[nodeId] = edgeList;
            }
            edgeList.Add(edge);
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
            Action<uint, SqliteDataReader> onRow, string whereClause = "")
        {
            // Fetch row count
            uint count = 0;
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT COUNT(*) as count FROM {tableName} {whereClause};";
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
                string query = $"SELECT * FROM {tableName} {whereClause} "
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
