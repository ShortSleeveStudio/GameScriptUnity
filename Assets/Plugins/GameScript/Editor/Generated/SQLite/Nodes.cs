// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class Nodes
    {
        public const string TABLE_NAME = "nodes";
        public long id { get; set; }
        public string name { get; set; }
        public long parent { get; set; }
        public long actor { get; set; }
        public long uiResponseText { get; set; }
        public long voiceText { get; set; }
        public long condition { get; set; }
        public long code { get; set; }
        public long codeOverride { get; set; }
        public bool isPreventResponse { get; set; }
        public string notes { get; set; }
        public bool isSystemCreated { get; set; }
        public string type { get; set; }
        public double positionX { get; set; }
        public double positionY { get; set; }

        public static Nodes FromReader(SqliteDataReader reader)
        {
            Nodes obj = new();
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
            obj.actor = reader.GetValue(3) is DBNull
                ? 0
                : reader.GetInt64(3)
                ;
            obj.uiResponseText = reader.GetValue(4) is DBNull
                ? 0
                : reader.GetInt64(4)
                ;
            obj.voiceText = reader.GetValue(5) is DBNull
                ? 0
                : reader.GetInt64(5)
                ;
            obj.condition = reader.GetValue(6) is DBNull
                ? 0
                : reader.GetInt64(6)
                ;
            obj.code = reader.GetValue(7) is DBNull
                ? 0
                : reader.GetInt64(7)
                ;
            obj.codeOverride = reader.GetValue(8) is DBNull
                ? 0
                : reader.GetInt64(8)
                ;
            obj.isPreventResponse = reader.GetValue(9) is DBNull
                ? false
                : reader.GetBoolean(9)
                ;
            obj.notes = reader.GetValue(10) is DBNull
                ? ""
                : reader.GetString(10)
                ;
            obj.isSystemCreated = reader.GetValue(11) is DBNull
                ? false
                : reader.GetBoolean(11)
                ;
            obj.type = reader.GetValue(12) is DBNull
                ? ""
                : reader.GetString(12)
                ;
            obj.positionX = reader.GetValue(13) is DBNull
                ? 0d
                : reader.GetDouble(13)
                ;
            obj.positionY = reader.GetValue(14) is DBNull
                ? 0d
                : reader.GetDouble(14)
                ;
            return obj;
        }
    }
}
