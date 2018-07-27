#region

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    public class ScriptableObjectUtility : ScriptableObject
    {
        public static string CinemachineInstallPath
        {
            get { return Path.GetFullPath(CinemachineInstallAssetPath); }
        }

        public static string CinemachineInstallAssetPath
        {
            get
            {
                ScriptableObject dummy = CreateInstance<ScriptableObjectUtility>();
                var path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy));
                DestroyImmediate(dummy);
                path = path.Substring(0, path.LastIndexOf("/Base"));
                return path;
            }
        }

        public static bool CinemachineIsPackage
        {
            get { return CinemachineInstallAssetPath.StartsWith("Packages"); }
        }

        public static bool AddDefineForAllBuildTargets(string k_Define)
        {
            var added = false;
            var targets = Enum.GetValues(typeof(BuildTargetGroup))
                .Cast<BuildTargetGroup>()
                .Where(x => x != BuildTargetGroup.Unknown)
                .Where(x => !BuildTargetIsObsolete(x));

            foreach (var target in targets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

                var list = defines.Split(';', ' ')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                if (!list.Contains(k_Define))
                {
                    list.Add(k_Define);
                    defines = list.Aggregate((a, b) => a + ";" + b);

                    PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
                    added = true;
                }
            }

            return added;
        }

        private static bool BuildTargetIsObsolete(BuildTargetGroup group)
        {
            var attrs = typeof(BuildTargetGroup)
                .GetField(group.ToString())
                .GetCustomAttributes(typeof(ObsoleteAttribute), false);

            return attrs != null && attrs.Length > 0;
        }

        public static void Create<T>(bool prependFolderName = false, bool trimName = true) where T : ScriptableObject
        {
            var className = typeof(T).Name;
            var assetName = className;
            var folder = GetSelectedAssetFolder();

            if (trimName)
            {
                var standardNames = new[] {"Asset", "Attributes", "Container"};
                assetName = standardNames.Aggregate(assetName,
                    (current, standardName) => current.Replace(standardName, ""));
            }

            if (prependFolderName)
            {
                var folderName = Path.GetFileName(folder);
                assetName = string.IsNullOrEmpty(assetName)
                    ? folderName
                    : string.Format("{0}_{1}", folderName, assetName);
            }

            Create(className, assetName, folder);
        }

        public static T CreateAt<T>(string assetPath) where T : ScriptableObject
        {
            var asset = CreateInstance<T>();
            if (asset == null)
            {
                Debug.LogError("failed to create instance of " + typeof(T).Name);
                return null;
            }

            AssetDatabase.CreateAsset(asset, assetPath);

            return asset;
        }

        private static ScriptableObject Create(string className, string assetName, string folder)
        {
            var asset = CreateInstance(className);
            if (asset == null)
            {
                Debug.LogError("failed to create instance of " + className);
                return null;
            }

            asset.name = assetName ?? className;

            var assetPath = GetUnusedAssetPath(folder, asset.name);
            AssetDatabase.CreateAsset(asset, assetPath);

            return asset;
        }

        private static string GetSelectedAssetFolder()
        {
            if (Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject))
            {
                var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                var assetPathAbsolute =
                    string.Format("{0}/{1}", Path.GetDirectoryName(Application.dataPath), assetPath);

                if (Directory.Exists(assetPathAbsolute))
                    return assetPath;
                return Path.GetDirectoryName(assetPath);
            }

            return "Assets";
        }

        private static string GetUnusedAssetPath(string folder, string assetName)
        {
            for (var n = 0; n < 9999; n++)
            {
                var assetPath = string.Format("{0}/{1}{2}.asset", folder, assetName, n == 0 ? "" : n.ToString());
                var existingGUID = AssetDatabase.AssetPathToGUID(assetPath);
                if (string.IsNullOrEmpty(existingGUID)) return assetPath;
            }

            return null;
        }
    }
}