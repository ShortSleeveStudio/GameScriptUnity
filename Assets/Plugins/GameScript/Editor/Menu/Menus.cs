using GameScript;
using UnityEditor;
using UnityEngine;

class Menus : MonoBehaviour
{
    [MenuItem("GameScript/Show Settings")]
    static void ShowSettings() => Selection.activeObject = Settings.Instance;
}
