// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class Conversations
    {
        public const string TABLE_NAME = "conversations";
        public long id { get; set; }
        public string name { get; set; }
        public bool isSystemCreated { get; set; }
        public string notes { get; set; }
        public bool isDeleted { get; set; }
        public bool isLayoutAuto { get; set; }
        public bool isLayoutVertical { get; set; }

        public static Conversations FromReader(SqliteDataReader reader)
        {
            Conversations obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.name = reader.GetValue(1) is DBNull
                ? ""
                : reader.GetString(1)
                ;
            obj.isSystemCreated = reader.GetValue(2) is DBNull
                ? false
                : reader.GetBoolean(2)
                ;
            obj.notes = reader.GetValue(3) is DBNull
                ? ""
                : reader.GetString(3)
                ;
            obj.isDeleted = reader.GetValue(4) is DBNull
                ? false
                : reader.GetBoolean(4)
                ;
            obj.isLayoutAuto = reader.GetValue(5) is DBNull
                ? false
                : reader.GetBoolean(5)
                ;
            obj.isLayoutVertical = reader.GetValue(6) is DBNull
                ? false
                : reader.GetBoolean(6)
                ;
            return obj;
        }
    }
}
