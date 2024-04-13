using UnityEditor;
using UnityEngine;

namespace GameScript
{
    class Menus : MonoBehaviour
    {
        #region Public
        [MenuItem("Tools/Show Settings")]
        static void ShowSettings() => Selection.activeObject = Settings.Instance;
        #endregion

        #region Internal Developement
        // private static readonly string k_DbCodeOutputDirectory = System.IO.Path.Combine(
        //     Application.dataPath,
        //     "Plugins",
        //     "GameScript",
        //     "Editor",
        //     "Generated",
        //     "SQLite"
        // );

        // [MenuItem("GameScript/Generate DB Files")]
        // static void GenerateDBFiles()
        // {
        //     DatabaseCodeGenerator.GenerateDatabaseCode(
        //         Settings.Instance.DatabasePath,
        //         k_DbCodeOutputDirectory
        //     );
        // }
        #endregion
    }
}
