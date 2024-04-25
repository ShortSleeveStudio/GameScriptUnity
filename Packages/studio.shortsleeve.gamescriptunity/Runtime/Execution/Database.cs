using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

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
                    throw new Exception("Must call Initialize() on database before use");
                return m_Instance;
            }
        }

        public static IEnumerator Initialize()
        {
            // Raw data
            byte[] binaryData;

            // Get relative path
            string relativePath = Path.Combine(
                Settings.Instance.ConversationDataPathRelative,
                RuntimeConstants.k_ConversationDataFilename
            );

#if UNITY_ANDROID && !UNITY_EDITOR
            string webPath = $"file://{Application.streamingAssetsPath}{relativePath}";

            // Load Web Request
            using (UnityWebRequest www = UnityWebRequest.Get(webPath))
            {
                // Wait for data
                yield return www.SendWebRequest();

                // Error Handling
                if (www.result != UnityWebRequest.Result.Success)
                    throw new Exception($"Failed to load {webPath}");

                // Compose Response
                binaryData = www.downloadHandler.data;
            }
#else
            binaryData = File.ReadAllBytes($"{Application.streamingAssetsPath}{relativePath}");
#endif

            // Decompress and deserialize conversation data
            BinaryFormatter serializer = new();
            using (MemoryStream dataStream = new MemoryStream(binaryData))
            {
                using (GZipStream zipStream = new(dataStream, CompressionMode.Decompress))
                {
                    m_Instance = (GameData)serializer.Deserialize(zipStream);
                }
            }
            yield break;
        }
        #endregion

        #region Static State
        private static Locale s_BinarySearchLocale = new();
        private static Localization s_BinarySearchLocalization = new();
        private static Conversation s_BinarySearchConversation = new();
        private static EmptyProperty s_BinarySearchProperty = new("");
        #endregion

        #region Static Methods
        public static Localization FindLocalization(uint localizationId) =>
            Find(localizationId, s_BinarySearchLocalization, Instance.Localizations);

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
