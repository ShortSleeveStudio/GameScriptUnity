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
        #region Constants
        private static string k_DBCodeOutputDirectory = Path.Combine(
            Application.dataPath, "Plugins", "GameScript", "Editor", "Generated", "SQLite");
        #endregion
        #region Variables 
        [SerializeField] private VisualTreeAsset m_SettingsXML;
        #endregion

        #region Unity Lifecycle Methods
        public override VisualElement CreateInspectorGUI()
        {
            // Create custom inspector 
            VisualElement root = new VisualElement();

            // Main Header
            Label mainHeader = new Label();
            mainHeader.name = "MainHeader";
            mainHeader.text = "GameScript Settings";

            // Max Conversations
            IntegerField maxConversations = new IntegerField();
            maxConversations.name = "MaxConversations";
            maxConversations.label = "Max Simultaneous Conversations";
            maxConversations.bindingPath = "MaxConversations";

            // Database Path 
            TextField databasePathField = new();
            databasePathField.name = "DatabasePath";
            databasePathField.label = "Database";
            databasePathField.bindingPath = "DatabasePath";
            databasePathField.SetEnabled(false);
            databasePathField.RegisterValueChangedCallback((ChangeEvent<string> change) =>
            {
                OnDbPathChanged(databasePathField, change.newValue);
            });

            // Select Database Button
            Button selectDatabase = new Button();
            selectDatabase.name = "SelectDatabase";
            selectDatabase.text = "Select Database";
            selectDatabase.clickable.clicked += () =>
            {
                // Set new path
                string path = EditorUtility.OpenFilePanel("Select Database", "", "");
                if (string.IsNullOrEmpty(path)) return;
                databasePathField.value = path;

                // Load database
                DatabaseImporter.ImportDatabase(path, k_DBCodeOutputDirectory);
            };

            // Set Initial Visibility for Database Path
            string databasePath = serializedObject.FindProperty("DatabasePath").stringValue;
            OnDbPathChanged(databasePathField, databasePath);

            // Default Inspector Foldout
            Foldout defaultInspectorFoldout = new Foldout();
            defaultInspectorFoldout.name = "DefaultInspector";
            defaultInspectorFoldout.text = "Default Inspector";
            InspectorElement.FillDefaultInspector(defaultInspectorFoldout, serializedObject, this);

            // Add All Elements
            root.Add(mainHeader);
            root.Add(maxConversations);
            root.Add(databasePathField);
            root.Add(selectDatabase);
            root.Add(defaultInspectorFoldout);
            return root;
        }

        private void OnDbPathChanged(TextField databasePathField, string newPath)
        {
            bool dbExists = File.Exists(newPath);
            ShowElement(databasePathField, dbExists);
        }

        private void ShowElement(BindableElement element, bool shouldShow)
        {
            element.style.display = shouldShow ? DisplayStyle.Flex : DisplayStyle.None;
        }
        #endregion
    }
}
