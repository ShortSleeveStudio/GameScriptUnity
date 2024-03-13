using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameScript
{
    public class Settings : ScriptableObject
    {
        #region Constants
        private static readonly string k_SettingsAsset
            = RuntimeConstants.SETTINGS_OBJECT_NAME + ".asset";
        private static readonly string k_SettingsAssetPath = Path.Combine(
            "Assets", "Plugins", "GameScript", "Runtime", "Resources", k_SettingsAsset);
        #endregion

        #region Singleton
        private static Settings m_Instance = null;
        public static Settings Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = Resources.Load(k_SettingsAsset) as Settings;
                    if (m_Instance == null)
                    {
#if UNITY_EDITOR
                        Settings asset = CreateInstance<Settings>();
                        // Set default values
                        asset.Version = RuntimeConstants.VERSION;
                        asset.MaxConversations = 16;
                        AssetDatabase.CreateAsset(asset, k_SettingsAssetPath);
                        AssetDatabase.SaveAssets();
                        m_Instance = asset;
#else
                        throw new Exception(
                            $"{RuntimeConstants.APP_NAME} settings ScriptableObject not found");
#endif
                    }
                }
                return m_Instance;
            }
        }
        #endregion

        #region Runtime Settings 
        public uint MaxFlags;
        public uint MaxSignals;
        public uint MaxConversations;
        public uint MaxScheduledBlocks;
        #endregion

        #region Editor Settings
        public string Version;
        public string DatabasePath;
        #endregion
    }
}
