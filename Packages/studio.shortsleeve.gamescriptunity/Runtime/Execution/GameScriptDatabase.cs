using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GameScript
{
    public class GameScriptDatabase
    {
        #region State
        private Locale m_BinarySearchLocale;
        private GameData m_GameData;
        private Localization m_BinarySearchLocalization;
        private Conversation m_BinarySearchConversation;
        private EmptyProperty m_BinarySearchProperty;
        #endregion

        #region Constructor
        internal GameScriptDatabase()
        {
            m_BinarySearchLocale = new();
            m_BinarySearchProperty = new("");
            m_BinarySearchLocalization = new();
            m_BinarySearchConversation = new();
        }
        #endregion

        #region Public API
        public GameData GameData => m_GameData;

        public Localization FindLocalization(uint localizationId) =>
            Find(localizationId, m_BinarySearchLocalization, m_GameData.Localizations);

        public Locale FindLocale(uint localeId) =>
            Find(localeId, m_BinarySearchLocale, m_GameData.Locales);

        public Conversation FindConversation(uint conversationId) =>
            Find(conversationId, m_BinarySearchConversation, m_GameData.Conversations);

        public Property FindProperty(Property[] properties, string propertyName)
        {
            m_BinarySearchProperty.SetName(propertyName);
            int index = Array.BinarySearch(properties, m_BinarySearchProperty);
            if (index == -1)
                return null;
            return properties[index];
        }
        #endregion

        #region Internal API
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        internal async Awaitable Initialize(Settings settings)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // Raw data
            byte[] binaryData;

            // Get relative path
            string relativePath = Path.Combine(
                settings.GameDataPathRelative,
                RuntimeConstants.k_ConversationDataFilename
            );

#if UNITY_ANDROID && !UNITY_EDITOR
            string webPath = $"file://{Application.streamingAssetsPath}{relativePath}";

            // Load Web Request
            using (
                UnityEngine.Networking.UnityWebRequest www =
                    UnityEngine.Networking.UnityWebRequest.Get(webPath)
            )
            {
                // Wait for data
                await www.SendWebRequest();

                // Error Handling
                if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
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
                    m_GameData = (GameData)serializer.Deserialize(zipStream);
                }
            }
        }
        #endregion

        #region Private API
        private T Find<T>(uint id, T searchBuddy, T[] arr)
            where T : BaseData<T>
        {
            searchBuddy.Id = id;
            int index = Array.BinarySearch(arr, searchBuddy);
            return arr[index];
        }
        #endregion
    }
}
