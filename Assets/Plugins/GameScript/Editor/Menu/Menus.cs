// using System.IO;
using System.IO;
using GameScript;
using UnityEditor;
using UnityEngine;

public class Menus : MonoBehaviour
{
    // TODO - this belongs somewhere else
    private static string sqliteDatabasePath = "C:\\Users\\emful\\Desktop\\DATABASE\\EXPORT_DATA\\HELPER_DB___DO_NOT_SHIP.db";
    private static string codeOutputDirectory = Path.Combine(Application.dataPath, "Routines");
    private static string dbCodeOutputDirectory = Path.Combine(Application.dataPath, "Plugins", "GameScript", "Editor", "Generated", "SQLite");

    [MenuItem("GameScript/Build All Routines", true)]
    static bool ValidateBuildAllRoutines()
    {
        // TODO - This belongs in another UI where you can surface the problem 

        if (!File.Exists(sqliteDatabasePath))
        {
            // throw new System.Exception($"Could not find SQLite at {sqliteDatabasePath}");
            return false;
        }
        if (!Directory.Exists(codeOutputDirectory))
        {
            // throw new System.Exception($"Could not find output directory at {codeOutputDirectory}");
            return false;
        }
        return true;
    }

    [MenuItem("GameScript/Build All Routines")]
    static void BuildAllRoutines()
    {
        Transpiler.Transpile(sqliteDatabasePath, codeOutputDirectory);
    }

    [MenuItem("GameScript/Import Database", true)]
    static bool ValidateImportDatabase()
    {
        // TODO - This belongs in another UI where you can surface the problem 
        if (!File.Exists(sqliteDatabasePath))
        {
            return false;
        }
        if (!Directory.Exists(dbCodeOutputDirectory))
        {
            return false;
        }
        return true;
    }

    // TODO - move this elsewhere
    [MenuItem("GameScript/Import Database")]
    static void ImportDatabase()
    {
        DatabaseImporter.ImportDatabase(sqliteDatabasePath, dbCodeOutputDirectory);
    }
}
