// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    class Routines
    {
        public const string TABLE_NAME = "routines";
        public long id;
        public string name;
        public string code;
        public long type;
        public bool isCondition;
        public string notes;
        public bool isSystemCreated;
        public long parent;

        public static Routines FromReader(SqliteDataReader reader)
        {
            Routines obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.name = reader.GetValue(1) is DBNull
                ? ""
                : reader.GetString(1)
                ;
            obj.code = reader.GetValue(2) is DBNull
                ? ""
                : reader.GetString(2)
                ;
            obj.type = reader.GetValue(3) is DBNull
                ? 0
                : reader.GetInt64(3)
                ;
            obj.isCondition = reader.GetValue(4) is DBNull
                ? false
                : reader.GetBoolean(4)
                ;
            obj.notes = reader.GetValue(5) is DBNull
                ? ""
                : reader.GetString(5)
                ;
            obj.isSystemCreated = reader.GetValue(6) is DBNull
                ? false
                : reader.GetBoolean(6)
                ;
            obj.parent = reader.GetValue(7) is DBNull
                ? 0
                : reader.GetInt64(7)
                ;
            return obj;
        }
    }
}
