using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameScript
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor
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
                    OnConversationDataPathChanged(conversationDataPath, change.newValue);
                }
            );

            // Conversation Path Button
            Button conversationDataButton = new Button();
            conversationDataButton.name = "ConversationDataButton";
            conversationDataButton.text = "Select Conversation Data Folder";
            conversationDataButton.clickable.clicked += () =>
            {
                string streamingAssetsPath = Application.streamingAssetsPath;
                if (!Directory.Exists(streamingAssetsPath))
                {
                    Directory.CreateDirectory(streamingAssetsPath);
                }
                string path = EditorUtility.OpenFolderPanel(
                    "Select Conversation Data Folder",
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
            #endregion

            #region Default Inspector
            // Default Inspector Foldout
            Foldout defaultInspectorFoldout = new Foldout();
            defaultInspectorFoldout.name = "DefaultInspector";
            defaultInspectorFoldout.text = "Default Inspector";
            InspectorElement.FillDefaultInspector(defaultInspectorFoldout, serializedObject, this);
            #endregion

            // Add All Elements
            root.Add(runtimeSettingsFoldout);
            root.Add(editorSettingsFoldout);
            root.Add(defaultInspectorFoldout);
            return root;
        }

        #region Event Handlers
        private void OnConversationDataPathChanged(TextField conversationDataPath, string newPath)
        {
            // Skip validating default path
            if (newPath == RuntimeConstants.k_DefaultStreamingAssetsPath)
                return;

            // Ensure valid path
            try
            {
                Path.GetFullPath(newPath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Conversation data folder was invalid. Resetting to default.");
                Debug.LogException(e);
                conversationDataPath.value = RuntimeConstants.k_DefaultStreamingAssetsPath;
                return;
            }

            // Ensure exists in streaming assets
            DirectoryInfo dir = new DirectoryInfo(newPath);
            DirectoryInfo streamingAssets = new DirectoryInfo(Application.streamingAssetsPath);
            bool isParent = false;
            while (dir.Parent != null)
            {
                if (dir.Parent.FullName == streamingAssets.FullName)
                {
                    isParent = true;
                    break;
                }
                else
                    dir = dir.Parent;
            }
            if (!isParent)
            {
                Debug.LogWarning(
                    "Conversation data must go in the StreamingAssets folder "
                        + "or a subfolder thereof. Resetting to default."
                );
                conversationDataPath.value = RuntimeConstants.k_DefaultStreamingAssetsPath;
            }
        }

        private void OnRoutinePathChanged(
            TextField routinePathField,
            Button databaseImportButton,
            string newPath
        )
        {
            if (Directory.Exists(newPath))
            {
                databaseImportButton.SetEnabled(true);
            }
            else
            {
                routinePathField.value = "";
                databaseImportButton.SetEnabled(false);
            }
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
        #endregion
    }
}
