using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameScript
{
    public class Database
    {
        #region Singleton
        private static GameData m_Instance;
        public static GameData Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    string path = Path.Combine(
                        Settings.Instance.ConversationDataPath,
                        RuntimeConstants.k_ConversationDataFilename
                    );
                    BinaryFormatter serializer = new();
                    using (FileStream fs = new(path, FileMode.Open))
                    {
                        using (GZipStream zipStream = new(fs, CompressionMode.Decompress))
                        {
                            m_Instance = (GameData)serializer.Deserialize(zipStream);
                        }
                    }
                }
                return m_Instance;
            }
        }
        #endregion

        #region Static State
        private static Conversation m_BinarySearchDummy = new();
        #endregion

        #region Static Methods
        public static Conversation FindConversation(uint conversationId)
        {
            GameData data = Instance;
            m_BinarySearchDummy.Id = conversationId;
            int index = Array.BinarySearch(data.Conversations, m_BinarySearchDummy);
            return data.Conversations[index];
        }
        #endregion
    }
}
