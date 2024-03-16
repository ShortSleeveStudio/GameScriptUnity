using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameScript
{
    [CustomEditor(typeof(Runner))]
    public class RunnerEditor : Editor
    {
        // TODO
        public override VisualElement CreateInspectorGUI()
        {
            // Create custom inspector
            VisualElement root = new VisualElement();

            // Add Default Inspector
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }
    }
}
