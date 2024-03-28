using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameScript
{
    public class Settings : ScriptableObject
    {
        #region Constants
        private static readonly string k_SettingsAsset =
            RuntimeConstants.k_SettingsAssetName + ".asset";
        #endregion

        #region Singleton
        private static Settings m_Instance = null;
        public static Settings Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = Resources.Load(RuntimeConstants.k_SettingsAssetName) as Settings;
                    if (m_Instance == null)
                    {
#if UNITY_EDITOR
                        string path = System.IO.Path.Combine(
                            "Assets",
                            "Plugins",
                            "GameScript",
                            "Runtime",
                            "Resources",
                            k_SettingsAsset
                        );
                        Settings asset = CreateInstance<Settings>();
                        // Set default values
                        asset.InitialConversationPool = 1;
                        asset.ConversationDataPath = RuntimeConstants.k_DefaultStreamingAssetsPath;
                        AssetDatabase.CreateAsset(asset, path);
                        AssetDatabase.SaveAssets();
                        m_Instance = asset;
#else
                        throw new System.Exception(
                            $"{RuntimeConstants.k_AppName} settings ScriptableObject not found"
                        );
#endif
                    }
                }
                return m_Instance;
            }
        }
        #endregion

        #region Runtime Settings
        public uint MaxFlags;
        public uint InitialConversationPool;
        #endregion

        #region Editor Settings
        public string DatabasePath;
        public string DatabaseVersion;
        public string RoutinePath;
        public string ConversationDataPath;
        #endregion
    }
}
