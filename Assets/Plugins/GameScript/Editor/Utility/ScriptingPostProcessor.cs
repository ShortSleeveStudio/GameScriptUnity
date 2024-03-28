using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace GameScript
{
    public class ScriptingPostProcessor : AssetPostprocessor
    {
        #region Constants
        private static readonly string k_GeneratedCodePath = Path.Combine(
            Application.dataPath,
            "Plugins",
            RuntimeConstants.k_AppName,
            "Editor",
            "Generated",
            "SQLite"
        );
        #endregion

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            // See if generated files exist
            bool generatedFilesExist = false;
            if (Directory.Exists(k_GeneratedCodePath))
            {
                string[] generatedFiles = Directory.GetFiles(k_GeneratedCodePath, "*.cs");
                generatedFilesExist = generatedFiles.Length > 0;
            }

            // Update scripting defines
            BuildTargetGroup target = BuildPipeline.GetBuildTargetGroup(
                EditorUserBuildSettings.activeBuildTarget
            );
            NamedBuildTarget namedTarget = NamedBuildTarget.FromBuildTargetGroup(target);
            if (generatedFilesExist)
            {
                PlayerSettings.SetScriptingDefineSymbols(
                    namedTarget,
                    RuntimeConstants.k_ScriptingSymbol
                );
            }
            else
            {
                string defines = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
                string[] defineSplits = defines.Split(';');
                List<string> definesSans = new();
                foreach (string define in defineSplits)
                {
                    if (define != RuntimeConstants.k_ScriptingSymbol)
                        definesSans.Add(define);
                }
                PlayerSettings.SetScriptingDefineSymbols(namedTarget, definesSans.ToArray());
            }
        }
    }
}
