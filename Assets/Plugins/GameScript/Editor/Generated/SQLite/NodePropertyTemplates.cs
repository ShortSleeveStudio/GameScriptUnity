// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    class NodePropertyTemplates
    {
        public const string TABLE_NAME = "node_property_templates";
        public long id;
        public string name;
        public long type;

        public static NodePropertyTemplates FromReader(SqliteDataReader reader)
        {
            NodePropertyTemplates obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.name = reader.GetValue(1) is DBNull
                ? ""
                : reader.GetString(1)
                ;
            obj.type = reader.GetValue(2) is DBNull
                ? 0
                : reader.GetInt64(2)
                ;
            return obj;
        }
    }
}
