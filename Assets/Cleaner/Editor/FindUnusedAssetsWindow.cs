#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#endregion

namespace AssetClean
{
    public class FindUnusedAssetsWindow : EditorWindow
    {
        private readonly AssetCollector collection = new AssetCollector();
        private readonly List<DeleteAsset> deleteAssets = new List<DeleteAsset>();
        private Vector2 scroll;

        [MenuItem("Window/Delete Unused Assets/only resource", false, 50)]
        private static void InitWithoutCode()
        {
            var window = CreateInstance<FindUnusedAssetsWindow>();
            window.collection.useCodeStrip = false;
            window.collection.Collection(new[] {"Assets"});
            window.CopyDeleteFileList(window.collection.deleteFileList);

            window.Show();
        }

        [MenuItem("Window/Delete Unused Assets/unused by editor", false, 51)]
        private static void InitWithout()
        {
            var window = CreateInstance<FindUnusedAssetsWindow>();
            window.collection.Collection(new[] {"Assets"});
            window.CopyDeleteFileList(window.collection.deleteFileList);

            window.Show();
        }

        [MenuItem("Window/Delete Unused Assets/unused by game", false, 52)]
        private static void Init()
        {
            var window = CreateInstance<FindUnusedAssetsWindow>();
            window.collection.saveEditorExtensions = false;
            window.collection.Collection(new[] {"Assets"});
            window.CopyDeleteFileList(window.collection.deleteFileList);

            window.Show();
        }

//		[MenuItem("Assets/Delete Unused Assets/unused by editor", false, 52)]
//		static void InitAssets ()
//		{
//			var paths = Selection.objects
//				.Select(c=>AssetDatabase.GetAssetPath(c))
//				.Where(c=>Directory.Exists(c));
//			if( paths.Any(c=> string.IsNullOrEmpty(c) ) ){
//				return;
//			}
//
//			var window = FindUnusedAssetsWindow.CreateInstance<FindUnusedAssetsWindow> ();
//			window.collection.Collection (paths.ToArray());
//			window.CopyDeleteFileList (window.collection.deleteFileList);
//			
//			window.Show ();
//		}
//
//		[MenuItem("Assets/Delete Unused Assets/unused by editor", true)]
//		static bool InitAssetsA ()
//		{
//			var paths = Selection.objects
//				.Select(c=>AssetDatabase.GetAssetPath(c))
//				.Where(c=>Directory.Exists(c));
//			return ! paths.Any(c=> string.IsNullOrEmpty(c) );
//		}


        [MenuItem("Assets/Delete Unused Assets/unused only resources", false, 52)]
        private static void InitAssetsOnlyResources()
        {
            var paths = Selection.objects
                .Select(c => AssetDatabase.GetAssetPath(c))
                .Where(c => Directory.Exists(c));
            if (paths.Any(c => string.IsNullOrEmpty(c))) return;

            var window = CreateInstance<FindUnusedAssetsWindow>();
            window.collection.useCodeStrip = false;
            window.collection.Collection(paths.ToArray());
            window.CopyDeleteFileList(window.collection.deleteFileList);

            window.Show();
        }

        [MenuItem("Assets/Delete Unused Assets/unused only resources", true)]
        private static bool InitAssetsOnlyResourcesA()
        {
            var paths = Selection.objects
                .Select(c => AssetDatabase.GetAssetPath(c))
                .Where(c => Directory.Exists(c));
            return !paths.Any(c => string.IsNullOrEmpty(c));
        }

        [MenuItem("Window/Delete Unused Assets/Clear cache")]
        private static void ClearCache()
        {
            File.Delete(AssetCollector.exportXMLPath);
            File.Delete(ClassReferenceCollection.xmlPath);

            EditorUtility.DisplayDialog("clear file", "clear file", "OK");
        }


        private void OnGUI()
        {
            using (var horizonal = new EditorGUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.LabelField("delete unreference assets from buildsettings and resources");
            }

            using (var scrollScope = new EditorGUILayout.ScrollViewScope(scroll))
            {
                scroll = scrollScope.scrollPosition;
                foreach (var asset in deleteAssets)
                {
                    if (string.IsNullOrEmpty(asset.path)) continue;

                    using (var horizonal = new EditorGUILayout.HorizontalScope())
                    {
                        asset.isDelete = EditorGUILayout.Toggle(asset.isDelete, GUILayout.Width(20));
                        var icon = AssetDatabase.GetCachedIcon(asset.path);
                        GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
                        if (GUILayout.Button(asset.path, EditorStyles.largeLabel))
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(asset.path);
                    }
                }
            }

            using (var horizonal = new EditorGUILayout.HorizontalScope("box"))
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Exclude from Project", GUILayout.Width(160)) && deleteAssets.Count != 0)
                    EditorApplication.delayCall += Exclude;
            }
        }

        private void Exclude()
        {
            RemoveFiles();
            Close();
        }

        private static void CleanDir()
        {
            RemoveEmptyDirectry("Assets");
            AssetDatabase.Refresh();
        }

        private void CopyDeleteFileList(IEnumerable<string> deleteFileList)
        {
            foreach (var asset in deleteFileList)
            {
                var filePath = AssetDatabase.GUIDToAssetPath(asset);
                if (string.IsNullOrEmpty(filePath) == false) deleteAssets.Add(new DeleteAsset {path = filePath});
            }
        }

        private void RemoveFiles()
        {
            try
            {
                var exportDirectry = "BackupUnusedAssets";
                Directory.CreateDirectory(exportDirectry);
                var files = deleteAssets.Where(item => item.isDelete).Select(item => item.path).ToArray();
                var backupPackageName = exportDirectry + "/package" + DateTime.Now.ToString("yyyyMMddHHmmss") +
                                        ".unitypackage";
                EditorUtility.DisplayProgressBar("export package", backupPackageName, 0);

                AssetDatabase.ExportPackage(files, backupPackageName);

                var i = 0;
                var length = deleteAssets.Count;

                foreach (var assetPath in files)
                {
                    i++;
                    EditorUtility.DisplayProgressBar("delete unused assets", assetPath, (float) i / length);
                    AssetDatabase.DeleteAsset(assetPath);
                    if (File.Exists(assetPath)) File.Delete(assetPath);
                }

                EditorUtility.DisplayProgressBar("clean directory", "", 1);
                foreach (var dir in Directory.GetDirectories("Assets")) RemoveEmptyDirectry(dir);

                Process.Start(exportDirectry);

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void RemoveEmptyDirectry(string path)
        {
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs) RemoveEmptyDirectry(dir);


            var files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly)
                .Where(item => Path.GetExtension(item) != ".meta");
            if (files.Count() == 0 && Directory.GetDirectories(path).Count() == 0)
            {
                var metaFile = AssetDatabase.GetTextMetaFilePathFromAssetPath(path);
                FileUtil.DeleteFileOrDirectory(path);
                FileUtil.DeleteFileOrDirectory(metaFile);
            }
        }

        private class DeleteAsset
        {
            public bool isDelete = true;
            public string path;
        }
    }
}