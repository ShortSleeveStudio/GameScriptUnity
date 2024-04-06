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
        private static Locale s_BinarySearchLocale = new();
        private static Conversation s_BinarySearchConversation = new();
        private static EmptyProperty s_BinarySearchProperty = new("");
        #endregion

        #region Static Methods
        public static Locale FindLocale(uint localeId) =>
            Find(localeId, s_BinarySearchLocale, Instance.Locales);

        public static Conversation FindConversation(uint conversationId) =>
            Find(conversationId, s_BinarySearchConversation, Instance.Conversations);

        public static Property FindProperty(Property[] properties, string propertyName)
        {
            s_BinarySearchProperty.SetName(propertyName);
            int index = Array.BinarySearch(properties, s_BinarySearchProperty);
            if (index == -1)
                return null;
            return properties[index];
        }

        private static T Find<T>(uint id, T searchBuddy, T[] arr)
            where T : BaseData<T>
        {
            searchBuddy.Id = id;
            int index = Array.BinarySearch(arr, searchBuddy);
            return arr[index];
        }
        #endregion
    }
}
