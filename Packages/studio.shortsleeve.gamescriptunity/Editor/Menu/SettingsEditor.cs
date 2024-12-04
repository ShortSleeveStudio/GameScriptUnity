using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameScript
{
    [CustomEditor(typeof(Settings))]
    class SettingsEditor : Editor
    {
        #region Variables
        [SerializeField]
        private VisualTreeAsset m_SettingsXML;
        #endregion

        public override VisualElement CreateInspectorGUI()
        {
            // Create custom inspector
            VisualElement root = new VisualElement();

            #region Runtime Settings
            // Max Conversations
            IntegerField maxConversations = new();
            maxConversations.name = "InitialConversationPool";
            maxConversations.label = "Initial Conversation Pool Size";
            maxConversations.tooltip =
                "This will decide how many simultaneous conversations can "
                + $"be run before {RuntimeConstants.k_AppName} needs to allocate more memory.";
            maxConversations.bindingPath = "InitialConversationPool";

            // Prevent Single Node Decisions
            Toggle preventSingleNodeChoices = new();
            preventSingleNodeChoices.name = "PreventSingleNodeChoices";
            preventSingleNodeChoices.label = "Prevent Single Node Choices";
            preventSingleNodeChoices.tooltip =
                "Normally, when a single node has UI Response Text set, it is treated as a "
                + "choice, even if it has no 'siblings'. This will prevent that behavior.";
            preventSingleNodeChoices.bindingPath = "PreventSingleNodeChoices";
            #endregion

            #region Runtime Settings Foldout
            // Runtime Settings Foldout
            Foldout runtimeSettingsFoldout = new();
            runtimeSettingsFoldout.name = "RuntimeSettings";
            runtimeSettingsFoldout.text = "Runtime Settings";
            runtimeSettingsFoldout.Add(maxConversations);
            runtimeSettingsFoldout.Add(preventSingleNodeChoices);
            #endregion

            #region Runtime Database
            // Runtime Database Header
            Label runtimeDatabaseHeader = new();
            runtimeDatabaseHeader.name = "RuntimeDatabaseHeader";
            runtimeDatabaseHeader.text = "Runtime Database";

            // Runtime Database Path Relative (not visible)
            TextField runtimeDatabasePathRelative = new();
            runtimeDatabasePathRelative.name = "RuntimeDatabasePathRelative";
            runtimeDatabasePathRelative.label = "Streaming Assets Sub-Folder Relative Path";
            runtimeDatabasePathRelative.bindingPath = "GameDataPathRelative";
            runtimeDatabasePathRelative.SetEnabled(false);

            // Runtime Database Path
            TextField runtimeDatabasePath = new();
            runtimeDatabasePath.name = "RuntimeDatabaseDataPath";
            runtimeDatabasePath.label = "Streaming Assets Sub-Folder";
            runtimeDatabasePath.bindingPath = "GameDataPath";
            runtimeDatabasePath.tooltip = "This is where the runtime game data will be stored.";
            runtimeDatabasePath.SetEnabled(false);
            runtimeDatabasePath.RegisterValueChangedCallback(
                (ChangeEvent<string> change) =>
                {
                    OnRuntimeDatabasePathChanged(
                        runtimeDatabasePath,
                        runtimeDatabasePathRelative,
                        change.newValue
                    );
                }
            );

            // Runtime Database Path Button
            Button conversationDataButton = new();
            conversationDataButton.name = "RuntimeDatabaseButton";
            conversationDataButton.text = "Select Folder";
            conversationDataButton.tooltip =
                "Select a folder within the StreamingAssets folder to place files exported by "
                + $"{RuntimeConstants.k_AppName} for use at runtime.";
            conversationDataButton.clickable.clicked += () =>
            {
                string streamingAssetsPath = Application.streamingAssetsPath;
                if (!Directory.Exists(streamingAssetsPath))
                {
                    Directory.CreateDirectory(streamingAssetsPath);
                }
                string path = EditorUtility.OpenFolderPanel(
                    "Select Streaming Assets Sub-Folder",
                    streamingAssetsPath,
                    RuntimeConstants.k_AppName
                );
                runtimeDatabasePath.value = path;
            };
            #endregion

            #region Editor Database
            // Editor Database Header
            Label databaseHeader = new();
            databaseHeader.name = "DatabaseHeader";
            databaseHeader.text = "Editor Database";

            // Editor Database Version
            TextField databaseVersionField = new();
            databaseVersionField.name = "DatabaseVersion";
            databaseVersionField.label = "Version";
            databaseVersionField.bindingPath = "DatabaseVersion";
            databaseVersionField.SetEnabled(false);
            databaseVersionField.RegisterValueChangedCallback(
                (ChangeEvent<string> change) =>
                {
                    OnDbVersionChanged(databaseVersionField, change.newValue);
                }
            );

            // Editor Database Path
            TextField databasePathField = new();
            databasePathField.name = "DatabasePath";
            databasePathField.label = "Database";
            databasePathField.bindingPath = "DatabasePath";
            databasePathField.SetEnabled(false);
            databasePathField.RegisterValueChangedCallback(
                (ChangeEvent<string> change) =>
                {
                    OnDbPathChanged(databasePathField, databaseVersionField, change.newValue);
                }
            );

            // Select Editor Database Button
            Button databaseImportButton = new();
            databaseImportButton.name = "ImportDatabase";
            databaseImportButton.text = "Import Database";
            databaseImportButton.tooltip =
                "Select a database file to import. This will prepare "
                + "all of your conversation data for runtime use and may take some time.";
            databaseImportButton.clickable.clicked += () =>
            {
                // Make sure import isn't in progress
                if (DatabaseImporter.IsImporting)
                {
                    EditorUtility.DisplayDialog(
                        "Import in Progress",
                        "Database import is in progress. Please wait for it to finish.",
                        "OK"
                    );
                    return;
                }

                // Set new path
                string dbPath = EditorUtility.OpenFilePanel("Import Database", "", "");
                if (string.IsNullOrEmpty(dbPath))
                    return;
                databasePathField.value = dbPath;

                // Load database
                string gameScriptOutputPath = serializedObject
                    .FindProperty("GeneratedPath")
                    .stringValue;
                string conversationOutputPath = serializedObject
                    .FindProperty("GameDataPath")
                    .stringValue;
                DatabaseImporter.ImportDatabase(
                    dbPath,
                    gameScriptOutputPath,
                    conversationOutputPath
                );
            };
            #endregion

            #region GameScript
            // Generated Header
            Label generatedHeader = new();
            generatedHeader.name = "GeneratedHeader";
            generatedHeader.text = "Generated";

            // Generated Output Path
            TextField generatedPathField = new();
            generatedPathField.name = "GeneratedPath";
            generatedPathField.label = "Code & References Output Folder";
            generatedPathField.bindingPath = "GeneratedPath";
            generatedPathField.SetEnabled(false);
            generatedPathField.RegisterValueChangedCallback(
                (ChangeEvent<string> change) =>
                {
                    OnGeneratedPathChanged(
                        generatedPathField,
                        databaseImportButton,
                        change.newValue
                    );
                }
            );

            // Select Generated Path Button
            Button generatedButton = new();
            generatedButton.name = $"{RuntimeConstants.k_AppName}PathButton";
            generatedButton.text = $"Select Folder";
            generatedButton.tooltip =
                "Select a folder within the Assets folder to place files generated by "
                + $"{RuntimeConstants.k_AppName}.";
            generatedButton.clickable.clicked += () =>
            {
                // Set new path
                string path = EditorUtility.OpenFolderPanel(
                    $"Select {RuntimeConstants.k_AppName} Folder",
                    "",
                    ""
                );
                if (string.IsNullOrEmpty(path))
                    return;
                generatedPathField.value = path;
            };
            #endregion

            #region Editor Settings Foldout
            Foldout editorSettingsFoldout = new();
            editorSettingsFoldout.name = "EditorSettings";
            editorSettingsFoldout.text = "Editor Settings";
            editorSettingsFoldout.Add(generatedHeader);
            editorSettingsFoldout.Add(generatedPathField);
            editorSettingsFoldout.Add(generatedButton);

            editorSettingsFoldout.Add(runtimeDatabaseHeader);
            editorSettingsFoldout.Add(runtimeDatabasePath);
            editorSettingsFoldout.Add(runtimeDatabasePathRelative);
            editorSettingsFoldout.Add(conversationDataButton);

            editorSettingsFoldout.Add(databaseHeader);
            editorSettingsFoldout.Add(databasePathField);
            editorSettingsFoldout.Add(databaseVersionField);
            editorSettingsFoldout.Add(databaseImportButton);
            #endregion

            #region Initialization
            // Set Initial Visibility for Database Path
            string generatedPath = serializedObject.FindProperty("GeneratedPath").stringValue;
            OnGeneratedPathChanged(generatedPathField, databaseImportButton, generatedPath);
            string databasePath = serializedObject.FindProperty("DatabasePath").stringValue;
            OnDbPathChanged(databasePathField, databaseVersionField, databasePath);
            string runtimeDatabasePathString = serializedObject
                .FindProperty("GameDataPath")
                .stringValue;
            OnRuntimeDatabasePathChanged(
                runtimeDatabasePath,
                runtimeDatabasePathRelative,
                runtimeDatabasePathString
            );
            #endregion

            #region Default Inspector
            // Default Inspector Foldout
            // Foldout defaultInspectorFoldout = new Foldout();
            // defaultInspectorFoldout.name = "DefaultInspector";
            // defaultInspectorFoldout.text = "Default Inspector";
            // InspectorElement.FillDefaultInspector(defaultInspectorFoldout, serializedObject, this);
            #endregion

            // Add All Elements
            root.Add(runtimeSettingsFoldout);
            root.Add(editorSettingsFoldout);
            // root.Add(defaultInspectorFoldout); // Used during developement
            return root;
        }

        #region Event Handlers
        private void OnRuntimeDatabasePathChanged(
            TextField runtimeDatabasePath,
            TextField runtimeDatabasePathRelative,
            string newPath
        )
        {
            // Skip validating default path
            if (newPath == RuntimeConstants.k_DefaultStreamingAssetsPath)
                goto Exit;

            // Ensure valid path
            if (!IsValidFolder(newPath))
            {
                Debug.LogWarning($"Runtime database folder was invalid. Resetting to default.");
                runtimeDatabasePath.value = RuntimeConstants.k_DefaultStreamingAssetsPath;
                goto Exit;
            }

            // Ensure exists in streaming assets
            if (!IsValidSubFolder(newPath, Application.streamingAssetsPath))
            {
                Debug.LogWarning(
                    "Runtime database must go in the StreamingAssets folder "
                        + "or a subfolder thereof. Resetting to default."
                );
                runtimeDatabasePath.value = RuntimeConstants.k_DefaultStreamingAssetsPath;
                goto Exit;
            }

            // Set Relative Path
            Exit:
            if (string.IsNullOrEmpty(runtimeDatabasePath.value))
                return;
            runtimeDatabasePathRelative.value = runtimeDatabasePath.value.Substring(
                Application.streamingAssetsPath.Length
            );
        }

        private void OnGeneratedPathChanged(
            TextField generatedPathField,
            Button databaseImportButton,
            string newPath
        )
        {
            // Ensure valid path
            if (!IsValidFolder(newPath))
            {
                generatedPathField.value = "";
                databaseImportButton.SetEnabled(false);
                return;
            }

            // Ensure exists in assets folder
            if (!IsValidSubFolder(newPath, Application.dataPath))
            {
                generatedPathField.value = "";
                databaseImportButton.SetEnabled(false);
                return;
            }

            // Enable database import
            databaseImportButton.SetEnabled(true);
        }

        private void OnDbPathChanged(
            TextField databasePathField,
            TextField databaseVersionField,
            string newPath
        )
        {
            if (File.Exists(newPath))
            {
                databaseVersionField.value = DatabaseImporter.GetDatabaseVersion(newPath);
                UnsetElementWarning(databasePathField);
            }
            else
            {
                databaseVersionField.value = "";
                if (!string.IsNullOrEmpty(newPath))
                {
                    SetElementWarning(databasePathField);
                }
                else
                {
                    UnsetElementWarning(databasePathField);
                }
            }
        }

        private void OnDbVersionChanged(TextField databaseVersionField, string newVersion)
        {
            if (newVersion == RuntimeConstants.k_Version || string.IsNullOrEmpty(newVersion))
            {
                UnsetElementWarning(databaseVersionField);
            }
            else
            {
                SetElementWarning(databaseVersionField);
            }
        }
        #endregion

        #region Helpers
        private void UnsetElementWarning(BindableElement element)
        {
            element.style.color = new(Color.white);
            element.style.backgroundColor = new(Color.clear);
        }

        private void SetElementWarning(BindableElement element)
        {
            element.style.color = new(Color.black);
            element.style.backgroundColor = new(Color.red);
        }

        private bool IsValidFolder(string path)
        {
            try
            {
                Path.GetFullPath(path);
                return Directory.Exists(path);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsValidSubFolder(string path, string parent)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo streamingAssets = new DirectoryInfo(parent);
            while (dir.Parent != null)
            {
                if (dir.Parent.FullName == streamingAssets.FullName)
                    return true;
                else
                    dir = dir.Parent;
            }
            return false;
        }
        #endregion
    }
}
