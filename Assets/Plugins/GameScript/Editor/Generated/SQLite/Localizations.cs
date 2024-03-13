// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class Localizations
    {
        public const string TABLE_NAME = "localizations";
        public long id { get; set; }
        public string name { get; set; }
        public long parent { get; set; }
        public bool isSystemCreated { get; set; }
        public string locale_1 { get; set; }
        public string locale_2 { get; set; }
        public string locale_3 { get; set; }

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
            obj.isSystemCreated = reader.GetValue(3) is DBNull
                ? false
                : reader.GetBoolean(3)
                ;
            obj.locale_1 = reader.GetValue(4) is DBNull
                ? ""
                : reader.GetString(4)
                ;
            obj.locale_2 = reader.GetValue(5) is DBNull
                ? ""
                : reader.GetString(5)
                ;
            obj.locale_3 = reader.GetValue(6) is DBNull
                ? ""
                : reader.GetString(6)
                ;
            return obj;
        }
    }
}
