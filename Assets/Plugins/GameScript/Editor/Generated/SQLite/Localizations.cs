// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    class Localizations
    {
        public const string TABLE_NAME = "localizations";
        public long id;
        public string name;
        public long parent;
        public bool is_system_created;
        public string locale_0;

        public static Localizations FromReader(SqliteDataReader reader)
        {
            Localizations obj = new();
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
            obj.is_system_created = reader.GetValue(3) is DBNull
                ? false
                : reader.GetBoolean(3)
                ;
            obj.locale_0 = reader.GetValue(4) is DBNull
                ? ""
                : reader.GetString(4)
                ;
            return obj;
        }
    }
}
