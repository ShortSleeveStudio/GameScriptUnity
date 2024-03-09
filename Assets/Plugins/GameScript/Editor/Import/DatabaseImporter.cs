using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine;
using static Database;

namespace GameScript
{
    public struct DatabaseColumn
    {
        public string name;
        public DatabaseType type;
    }

    public struct RoutineTypeData
    {
        public int id;
        public string name;
    }

    public enum DatabaseType
    {
        TEXT,
        DECIMAL,
        INTEGER,
        BOOLEAN,
    }

    public static class DatabaseImporter
    {
        public static async void ImportDatabase(string sqliteDatabasePath, string outputDirectory)
        {
            await Task.Run(() => ImportDatabaseAsync(sqliteDatabasePath, outputDirectory));
            AssetDatabase.Refresh();
        }

        static void ImportDatabaseAsync(string sqliteDatabasePath, string outputDirectory)
        {
            int progressId = 0;
            try
            {
                // Start Progress
                progressId = Progress.Start($"Importing {Constants.APP_NAME} database");

                // Delete old files
                if (Directory.Exists(outputDirectory)) Directory.Delete(outputDirectory, true);
                Directory.CreateDirectory(outputDirectory);

                // Connect to database
                bool foundRoutineTypes = false;
                List<string> tableNames = new();
                Dictionary<string, List<DatabaseColumn>> tableToColumns = new();
                using (SqliteConnection connection
                    = new(Database.SQLitePathToURI(sqliteDatabasePath)))
                {
                    // Open connection
                    connection.Open();

                    // Fetch table names
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tableName = reader.GetString(0);
                                if (tableName != "sqlite_sequence") tableNames.Add(tableName);
                            }
                        }
                    }

                    // Fetch table schema
                    Progress.Report(progressId, 0.33f, "Loading column types");
                    for (int i = 0; i < tableNames.Count; i++)
                    {
                        string tableName = tableNames[i];
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            // Add to map
                            List<DatabaseColumn> columns = new();
                            tableToColumns.Add(tableName, columns);

                            // Lookup columns
                            command.CommandType = CommandType.Text;
                            command.CommandText = $"pragma table_info({tableName});";
                            using (SqliteDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Column name
                                    string columnName = reader.GetString(1);
                                    bool startsWithIs = columnName.StartsWith("is"); // it's a bool
                                    // Column type
                                    string columnType = reader.GetString(2);
                                    DatabaseType type;
                                    switch (columnType)
                                    {
                                        case "INTEGER":
                                            type = startsWithIs
                                                ? DatabaseType.BOOLEAN
                                                : DatabaseType.INTEGER
                                                ;
                                            break;
                                        case "TEXT":
                                            type = DatabaseType.TEXT;
                                            break;
                                        case "NUMERIC":
                                            type = DatabaseType.DECIMAL;
                                            break;
                                        default:
                                            throw new Exception(
                                                $"Encountered unknown database type: {columnType}");
                                    }
                                    columns.Add(new DatabaseColumn()
                                    {
                                        name = columnName,
                                        type = type,
                                    });
                                }
                            }
                        }
                        if (tableName == Constants.ROUTINE_TYPES_TABLE_NAME)
                        {
                            foundRoutineTypes = true;
                            List<RoutineTypeData> routineTypes = new();
                            using (SqliteCommand command = connection.CreateCommand())
                            {
                                command.CommandType = CommandType.Text;
                                command.CommandText = $"SELECT id, name "
                                    + $"FROM {Constants.ROUTINE_TYPES_TABLE_NAME};"
                                    ;
                                using (SqliteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int routineTypeId = reader.GetInt32(0);
                                        string routineTypeName = reader.GetString(1);
                                        routineTypes.Add(new RoutineTypeData()
                                        {
                                            id = routineTypeId,
                                            name = routineTypeName,
                                        });
                                    }
                                }
                            }
                            GenerateRoutineTypes(routineTypes, outputDirectory);
                        }
                    }
                }

                // Ensure routine types were found/generated
                if (!foundRoutineTypes) throw new Exception("Could not find routine type table");

                // Generate types
                Progress.Report(progressId, 0.66f, "Generating type files");
                GenerateTypes(tableToColumns, outputDirectory);
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

        static void GenerateRoutineTypes(List<RoutineTypeData> routineTypes, string outputDirectory)
        {
            using (StreamWriter writer = new StreamWriter(
                Path.Combine(outputDirectory, "RoutineType.cs")))
            {
                WriteLine(writer, 0, $"// {Constants.GENERATED_CODE_WARNING}");
                WriteLine(writer, 0, "");
                WriteLine(writer, 0, $"namespace {Constants.APP_NAME}");
                WriteLine(writer, 0, "{");
                WriteLine(writer, 1, $"public enum RoutineType");
                WriteLine(writer, 1, "{");
                for (int i = 0; i < routineTypes.Count; i++)
                {
                    RoutineTypeData data = routineTypes[i];
                    WriteLine(writer, 2, $"{data.name} = {data.id},");
                }
                WriteLine(writer, 1, "}");
                WriteLine(writer, 0, "}");
            }
        }

        static void GenerateTypes(
            Dictionary<string, List<DatabaseColumn>> tableToColumns, string outputDirectory)
        {
            // Generate new files
            foreach (KeyValuePair<string, List<DatabaseColumn>> entry in tableToColumns)
            {
                string friendlyTableName = PascalCase(entry.Key);
                using (StreamWriter writer = new StreamWriter(
                    Path.Combine(outputDirectory, $"{friendlyTableName}.cs")))
                {
                    WriteLine(writer, 0, "// GENERATED CODE - DO NOT EDIT BY HAND");
                    WriteLine(writer, 0, "");
                    WriteLine(writer, 0, "using System;");
                    WriteLine(writer, 0, "using Mono.Data.Sqlite;");
                    WriteLine(writer, 0, "");
                    WriteLine(writer, 0, $"namespace {Constants.APP_NAME}");
                    WriteLine(writer, 0, "{");
                    WriteLine(writer, 1, $"public class {friendlyTableName}");
                    WriteLine(writer, 1, "{");
                    // Table Name
                    WriteLine(writer, 2, $"public const string TABLE_NAME = \"{entry.Key}\";");
                    // Fields
                    for (int i = 0; i < entry.Value.Count; i++)
                    {
                        DatabaseColumn column = entry.Value[i];
                        string columnType = DatabaseTypeToTypeString(column.type);
                        WriteLine(writer, 2, $"public {columnType} {column.name} {{ get; set; }}");
                    }
                    WriteLine(writer, 0, "");
                    // Deserializer
                    WriteLine(writer, 2,
                        $"public static {friendlyTableName} FromReader(SqliteDataReader reader)");
                    WriteLine(writer, 2, "{");
                    WriteLine(writer, 3, $"{friendlyTableName} obj = new();");
                    for (int i = 0; i < entry.Value.Count; i++)
                    {
                        DatabaseColumn column = entry.Value[i];
                        string readerMethod = DatabaseTypeToReaderMethod(column.type);
                        WriteLine(writer, 3,
                            $"obj.{column.name} = reader.GetValue({i}) is DBNull");
                        WriteLine(writer, 4, $"? {DatabaseTypeToDefaultValue(column.type)}");
                        WriteLine(writer, 4, $": reader.{readerMethod}({i})");
                        WriteLine(writer, 4, ";");
                    }
                    WriteLine(writer, 3, "return obj;");
                    WriteLine(writer, 2, "}");
                    WriteLine(writer, 1, "}");
                    WriteLine(writer, 0, "}");
                }
            }
        }

        static string DatabaseTypeToDefaultValue(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.TEXT:
                    return "\"\"";
                case DatabaseType.DECIMAL:
                    return "0d";
                case DatabaseType.INTEGER:
                    return "0";
                case DatabaseType.BOOLEAN:
                    return "false";
                default:
                    throw new Exception($"Unknown database type encountered: {type}");
            }
        }

        static string DatabaseTypeToReaderMethod(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.TEXT:
                    return "GetString";
                case DatabaseType.DECIMAL:
                    return "GetDouble";
                case DatabaseType.INTEGER:
                    return "GetInt64";
                case DatabaseType.BOOLEAN:
                    return "GetBoolean";
                default:
                    throw new Exception($"Unknown database type encountered: {type}");
            }
        }

        static string DatabaseTypeToTypeString(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.TEXT:
                    return "string";
                case DatabaseType.DECIMAL:
                    return "double";
                case DatabaseType.INTEGER:
                    return "long";
                case DatabaseType.BOOLEAN:
                    return "bool";
                default:
                    throw new Exception($"Unknown database type encountered: {type}");
            }
        }

        static string PascalCase(string word)
        {
            return string.Join("", word.Split('_')
                         .Select(w => w.Trim())
                         .Where(w => w.Length > 0)
                         .Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1).ToLower()));
        }
    }
}