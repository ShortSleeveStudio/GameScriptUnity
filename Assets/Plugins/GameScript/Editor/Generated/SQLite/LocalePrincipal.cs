// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class LocalePrincipal
    {
        public const string TABLE_NAME = "locale_principal";
        public long id { get; set; }
        public long principal { get; set; }

        public static LocalePrincipal FromReader(SqliteDataReader reader)
        {
            LocalePrincipal obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.principal = reader.GetValue(1) is DBNull
                ? 0
                : reader.GetInt64(1)
                ;
            return obj;
        }
    }
}
