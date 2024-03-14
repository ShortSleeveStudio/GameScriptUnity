// GENERATED CODE - DO NOT EDIT BY HAND

using System;
using Mono.Data.Sqlite;

namespace GameScript
{
    public class Nodes
    {
        public const string TABLE_NAME = "nodes";
        public long id;
        public long parent;
        public long actor;
        public long uiResponseText;
        public long voiceText;
        public long condition;
        public long code;
        public long codeOverride;
        public bool isPreventResponse;
        public string notes;
        public bool isSystemCreated;
        public string type;
        public double positionX;
        public double positionY;

        public static Nodes FromReader(SqliteDataReader reader)
        {
            Nodes obj = new();
            obj.id = reader.GetValue(0) is DBNull
                ? 0
                : reader.GetInt64(0)
                ;
            obj.parent = reader.GetValue(1) is DBNull
                ? 0
                : reader.GetInt64(1)
                ;
            obj.actor = reader.GetValue(2) is DBNull
                ? 0
                : reader.GetInt64(2)
                ;
            obj.uiResponseText = reader.GetValue(3) is DBNull
                ? 0
                : reader.GetInt64(3)
                ;
            obj.voiceText = reader.GetValue(4) is DBNull
                ? 0
                : reader.GetInt64(4)
                ;
            obj.condition = reader.GetValue(5) is DBNull
                ? 0
                : reader.GetInt64(5)
                ;
            obj.code = reader.GetValue(6) is DBNull
                ? 0
                : reader.GetInt64(6)
                ;
            obj.codeOverride = reader.GetValue(7) is DBNull
                ? 0
                : reader.GetInt64(7)
                ;
            obj.isPreventResponse = reader.GetValue(8) is DBNull
                ? false
                : reader.GetBoolean(8)
                ;
            obj.notes = reader.GetValue(9) is DBNull
                ? ""
                : reader.GetString(9)
                ;
            obj.isSystemCreated = reader.GetValue(10) is DBNull
                ? false
                : reader.GetBoolean(10)
                ;
            obj.type = reader.GetValue(11) is DBNull
                ? ""
                : reader.GetString(11)
                ;
            obj.positionX = reader.GetValue(12) is DBNull
                ? 0d
                : reader.GetDouble(12)
                ;
            obj.positionY = reader.GetValue(13) is DBNull
                ? 0d
                : reader.GetDouble(13)
                ;
            return obj;
        }
    }
}
