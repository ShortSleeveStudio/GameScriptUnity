// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class Edges
    {
        public const string TABLE_NAME = "edges";
        public long id { get; set; }
        public string name { get; set; }
        public long parent { get; set; }
        public long priority { get; set; }
        public string notes { get; set; }
        public long type { get; set; }
        public long source { get; set; }
        public long target { get; set; }

        public static Edges FromReader(SqliteDataReader reader)
        {
            Edges obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.name = reader.GetValue(1) is DBNull
                ? ""
                : reader.GetString(1)
                ;
            obj.parent = reader.GetValue(2) is DBNull
                ? 0
                : reader.GetInt64(2)
                ;
            obj.priority = reader.GetValue(3) is DBNull
                ? 0
                : reader.GetInt64(3)
                ;
            obj.notes = reader.GetValue(4) is DBNull
                ? ""
                : reader.GetString(4)
                ;
            obj.type = reader.GetValue(5) is DBNull
                ? 0
                : reader.GetInt64(5)
                ;
            obj.source = reader.GetValue(6) is DBNull
                ? 0
                : reader.GetInt64(6)
                ;
            obj.target = reader.GetValue(7) is DBNull
                ? 0
                : reader.GetInt64(7)
                ;
            return obj;
        }
    }
}
