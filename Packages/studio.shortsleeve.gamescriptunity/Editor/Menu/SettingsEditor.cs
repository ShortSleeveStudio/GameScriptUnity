using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
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
            IntegerField maxConversations = new IntegerField();
            maxConversations.name = "InitialConversationPool";
            maxConversations.label = "Initial Conversation Pool Size";
            maxConversations.tooltip =
                "This will decide how many simultaneous conversations can "
                + $"be run before {RuntimeConstants.k_AppName} needs to allocate more memory.";
            maxConversations.bindingPath = "InitialConversationPool";
            #endregion

            #region Runtime Settings Foldout
            // Runtime Settings Foldout
            Foldout runtimeSettingsFoldout = new Foldout();
            runtimeSettingsFoldout.name = "RuntimeSettings";
            runtimeSettingsFoldout.text = "Runtime Settings";
            runtimeSettingsFoldout.Add(maxConversations);
            #endregion

            #region Streaming Assets
            // Conversations Header
            Label conversationDataHeader = new Label();
            conversationDataHeader.name = "ConversationDataHeader";
            conversationDataHeader.text = "Conversations";

            // Conversation Data Path Relative (not visible)
            TextField conversationDataPathRelative = new();
            conversationDataPathRelative.name = "ConversationDataPathRelative";
            conversationDataPathRelative.label = "Streaming Assets Sub-Folder Relative Path";
            conversationDataPathRelative.bindingPath = "ConversationDataPathRelative";
            conversationDataPathRelative.SetEnabled(false);

            // Conversation Data Path
            TextField conversationDataPath = new();
            conversationDataPath.name = "ConversationDataPath";
            conversationDataPath.label = "Streaming Assets Sub-Folder";
            conversationDataPath.bindingPath = "ConversationDataPath";
            conversationDataPath.tooltip = "This is where conversation data will be stored.";
            conversationDataPath.SetEnabled(false);
            conversationDataPath.RegisterValueChangedCallback(
                (ChangeEvent<string> change) =>
                {
                    OnConversationDataPathChanged(
                        conversationDataPath,
                        conversationDataPathRelative,
                        change.newValue
                    );
                }
            );

            // Conversation Path Button
            Button conversationDataButton = new Button();
            conversationDataButton.name = "ConversationDataButton";
            conversationDataButton.text = "Select Streaming Assets Sub-Folder";
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
                conversationDataPath.value = path;
            };
            #endregion

            #region Database
            // Database Header
            Label databaseHeader = new Label();
            databaseHeader.name = "DatabaseHeader";
            databaseHeader.text = "Database";

            // Database Version
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

            // Database Path
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

            // Select Database Button
            Button databaseImportButton = new Button();
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
                string routineOutputPath = serializedObject.FindProperty("RoutinePath").stringValue;
                string conversationOutputPath = serializedObject
                    .FindProperty("ConversationDataPath")
                    .stringValue;
                DatabaseImporter.ImportDatabase(dbPath, routineOutputPath, conversationOutputPath);
            };
            #endregion

            #region Routines
            // Routine Header
            Label routineHeader = new Label();
            routineHeader.name = "RoutineHeader";
            routineHeader.text = "Routines";

            // Routine Output Path
            TextField routinePathField = new();
            routinePathField.name = "RoutinePath";
            routinePathField.label = "Routine Output Folder";
            routinePathField.bindingPath = "RoutinePath";
            routinePathField.SetEnabled(false);
            routinePathField.RegisterValueChangedCallback(
                (ChangeEvent<string> change) =>
                {
                    OnRoutinePathChanged(routinePathField, databaseImportButton, change.newValue);
                }
            );

            // Select Routine Path Button
            Button routinePathButton = new Button();
            routinePathButton.name = "RoutinePathButton";
            routinePathButton.text = "Select Routine Folder";
            routinePathButton.tooltip =
                "Select a folder within the Assets folder to place files generated by "
                + $"{RuntimeConstants.k_AppName}.";
            routinePathButton.clickable.clicked += () =>
            {
                // Set new path
                string path = EditorUtility.OpenFolderPanel("Select Routine Folder", "", "");
                if (string.IsNullOrEmpty(path))
                    return;
                routinePathField.value = path;
            };
            #endregion

            #region Editor Settings Foldout
            Foldout editorSettingsFoldout = new Foldout();
            editorSettingsFoldout.name = "EditorSettings";
            editorSettingsFoldout.text = "Editor Settings";
            editorSettingsFoldout.Add(routineHeader);
            editorSettingsFoldout.Add(routinePathField);
            editorSettingsFoldout.Add(routinePathButton);

            editorSettingsFoldout.Add(conversationDataHeader);
            editorSettingsFoldout.Add(conversationDataPath);
            editorSettingsFoldout.Add(conversationDataPathRelative);
            editorSettingsFoldout.Add(conversationDataButton);

            editorSettingsFoldout.Add(databaseHeader);
            editorSettingsFoldout.Add(databasePathField);
            editorSettingsFoldout.Add(databaseVersionField);
            editorSettingsFoldout.Add(databaseImportButton);
            #endregion

            #region Initialization
            // Set Initial Visibility for Database Path
            string routinePath = serializedObject.FindProperty("RoutinePath").stringValue;
            OnRoutinePathChanged(routinePathField, databaseImportButton, routinePath);
            string databasePath = serializedObject.FindProperty("DatabasePath").stringValue;
            OnDbPathChanged(databasePathField, databaseVersionField, databasePath);
            string conversationDataPathString = serializedObject
                .FindProperty("ConversationDataPath")
                .stringValue;
            OnConversationDataPathChanged(
                conversationDataPath,
                conversationDataPathRelative,
                conversationDataPathString
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
        private void OnConversationDataPathChanged(
            TextField conversationDataPath,
            TextField conversationDataPathRelative,
            string newPath
        )
        {
            // Skip validating default path
            if (newPath == RuntimeConstants.k_DefaultStreamingAssetsPath)
                goto Exit;

            // Ensure valid path
            if (!IsValidFolder(newPath))
            {
                Debug.LogWarning($"Conversation data folder was invalid. Resetting to default.");
                conversationDataPath.value = RuntimeConstants.k_DefaultStreamingAssetsPath;
                goto Exit;
            }

            // Ensure exists in streaming assets
            if (!IsValidSubFolder(newPath, Application.streamingAssetsPath))
            {
                Debug.LogWarning(
                    "Conversation data must go in the StreamingAssets folder "
                        + "or a subfolder thereof. Resetting to default."
                );
                conversationDataPath.value = RuntimeConstants.k_DefaultStreamingAssetsPath;
                goto Exit;
            }

            // Set Relative Path
            Exit:
            if (string.IsNullOrEmpty(conversationDataPath.value))
                return;
            conversationDataPathRelative.value = conversationDataPath.value.Substring(
                Application.streamingAssetsPath.Length
            );
        }

        private void OnRoutinePathChanged(
            TextField routinePathField,
            Button databaseImportButton,
            string newPath
        )
        {
            // Ensure valid path
            if (!IsValidFolder(newPath))
            {
                Debug.LogWarning($"Routine folder folder was invalid.");
                routinePathField.value = "";
                databaseImportButton.SetEnabled(false);
                return;
            }

            // Ensure exists in assets folder
            if (!IsValidSubFolder(newPath, Application.dataPath))
            {
                Debug.LogWarning(
                    "Routine folder must be in the Assets folder " + "or a subfolder thereof."
                );
                routinePathField.value = "";
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
