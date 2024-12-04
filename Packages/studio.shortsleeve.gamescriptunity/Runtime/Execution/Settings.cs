using UnityEngine;

namespace GameScript
{
    public class Settings : ScriptableObject
    {
        #region Runtime Settings
        public uint MaxFlags;
        public uint InitialConversationPool;
        public bool PreventSingleNodeChoices;
        #endregion

        #region Editor Settings
        public string DatabasePath;
        public string DatabaseVersion;
        public string GeneratedPath;
        public string GameDataPath;
        public string GameDataPathRelative;
        #endregion

        #region Editor
#if UNITY_EDITOR
        public static Settings GetSettings()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameScript.Settings", null);
            Settings settings;
            // Delete extras
            if (guids.Length > 1)
            {
                for (int i = 1; i < guids.Length; i++)
                {
                    string pathToDelete = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                    Debug.LogWarning($"Deleting extra GameScript settings object: {pathToDelete}");
                    UnityEditor.AssetDatabase.DeleteAsset(pathToDelete);
                }
            }

            // At least one valid settings object
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                settings = UnityEditor.AssetDatabase.LoadAssetAtPath<Settings>(path);
            }
            // Create if non-existant
            else
            {
                settings = CreateInstance<Settings>();
                settings.InitialConversationPool = 1;
                settings.GameDataPath = RuntimeConstants.k_DefaultStreamingAssetsPath;
                UnityEditor.AssetDatabase.CreateAsset(
                    settings,
                    $"Assets/{RuntimeConstants.k_SettingsAssetName}.asset"
                );
                UnityEditor.AssetDatabase.SaveAssets();
            }

            return settings;
        }
#endif
        #endregion
    }
}
