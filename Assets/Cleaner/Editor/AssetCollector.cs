#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEditor;

#endregion

namespace AssetClean
{
    public class AssetCollector
    {
        public static readonly string exportXMLPath = "referencemap.xml";

        public List<string> deleteFileList = new List<string>();
        private List<CollectionData> referenceCollection = new List<CollectionData>();
        public bool saveEditorExtensions = true;

        public bool useCodeStrip = true;

        public void Collection(string[] collectionFolders)
        {
            try
            {
                var serialize = new XmlSerializer(typeof(List<CollectionData>));
                deleteFileList.Clear();
                referenceCollection.Clear();

                if (File.Exists(exportXMLPath))
                    using (var fileStream = new StreamReader(exportXMLPath))
                    {
                        referenceCollection = (List<CollectionData>) serialize.Deserialize(fileStream);
                        fileStream.Close();
                    }

                var collectionList = new List<IReferenceCollection>();

                if (useCodeStrip) collectionList.Add(new ClassReferenceCollection(saveEditorExtensions));

                collectionList.AddRange(new IReferenceCollection[]
                {
                    new ShaderReferenceCollection(),
                    new AssetReferenceCollection()
                });

                foreach (var collection in collectionList)
                {
                    collection.Init(referenceCollection);
                    collection.CollectionFiles();
                }

                // Find assets
                var files = StripTargetPathsAll(useCodeStrip, collectionFolders);

                foreach (var path in files)
                {
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    deleteFileList.Add(guid);
                }

                EditorUtility.DisplayProgressBar("checking", "collection all files", 0.2f);
                UnregistReferenceFromResources();

                EditorUtility.DisplayProgressBar("checking", "check reference from resources", 0.4f);
                UnregistReferenceFromScenes();

                EditorUtility.DisplayProgressBar("checking", "check reference from scenes", 0.6f);
                if (saveEditorExtensions) UnregistEditorCodes();

                EditorUtility.DisplayProgressBar("checking", "check reference from ignorelist", 0.8f);
                UnregistReferenceFromIgnoreList();
                UnregistReferenceFromExtensionMethod();

                using (var fileStream = new StreamWriter(exportXMLPath))
                {
                    serialize.Serialize(fileStream, referenceCollection);
                    fileStream.Close();
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private List<string> StripTargetPathsAll(bool isUseCodeStrip, string[] pathes)
        {
            var files = pathes.SelectMany(c => Directory.GetFiles(c, "*.*", SearchOption.AllDirectories))
                .Distinct()
                .Where(item => Path.GetExtension(item) != ".meta")
                .Where(item => Path.GetExtension(item) != ".js")
                .Where(item => Path.GetExtension(item) != ".dll")
                .Where(item => Regex.IsMatch(item, "[\\/\\\\]Gizmos[\\/\\\\]") == false)
                .Where(item => Regex.IsMatch(item, "[\\/\\\\]Plugins[\\/\\\\]Android[\\/\\\\]") == false)
                .Where(item => Regex.IsMatch(item, "[\\/\\\\]Plugins[\\/\\\\]iOS[\\/\\\\]") == false)
                .Where(item => Regex.IsMatch(item, "[\\/\\\\]Resources[\\/\\\\]") == false);

            if (isUseCodeStrip == false) files = files.Where(item => Path.GetExtension(item) != ".cs");

            return files.ToList();
        }

        private void UnregistReferenceFromIgnoreList()
        {
            var codePaths = deleteFileList.Where(fileName => Path.GetExtension(fileName) == ".cs");

            foreach (var path in codePaths)
            {
                var code = ClassReferenceCollection.StripComment(File.ReadAllText(path));
                if (Regex.IsMatch(code, "static\\s*(partial)*\\s*class"))
                    UnregistFromDelteList(AssetDatabase.AssetPathToGUID(path));
            }
        }

        private void UnregistReferenceFromExtensionMethod()
        {
            var resourcesFiles = deleteFileList
                .Where(item => Path.GetExtension(item) != ".meta")
                .ToArray();
            foreach (var path in AssetDatabase.GetDependencies(resourcesFiles))
                UnregistFromDelteList(AssetDatabase.AssetPathToGUID(path));
        }

        private void UnregistReferenceFromResources()
        {
            var resourcesFiles = deleteFileList
                .Where(item => Regex.IsMatch(item, "[\\/\\\\]Resources[\\/\\\\]"))
                .Where(item => Path.GetExtension(item) != ".meta")
                .ToArray();
            foreach (var path in AssetDatabase.GetDependencies(resourcesFiles))
                UnregistFromDelteList(AssetDatabase.AssetPathToGUID(path));
        }

        private void UnregistReferenceFromScenes()
        {
            // Exclude objects that reference from scenes.
            var scenes = EditorBuildSettings.scenes
                .Where(item => item.enabled)
                .Select(item => item.path)
                .ToArray();
            foreach (var path in AssetDatabase.GetDependencies(scenes))
                UnregistFromDelteList(AssetDatabase.AssetPathToGUID(path));
        }

        private void UnregistEditorCodes()
        {
            // Exclude objects that reference from Editor API
            var editorcodes = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories)
                .Where(fileName => Path.GetExtension(fileName) == ".cs")
                .Where(item => Regex.IsMatch(item, "[\\/\\\\]Editor[\\/\\\\]"))
                .ToArray();

            EditorUtility.DisplayProgressBar("checking", "check reference from editor codes", 0.8f);

            foreach (var path in editorcodes)
            {
                var code = ClassReferenceCollection.StripComment(File.ReadAllText(path));
                if (Regex.IsMatch(code, "(\\[MenuItem|AssetPostprocessor|EditorWindow)"))
                    UnregistFromDelteList(AssetDatabase.AssetPathToGUID(path));
            }

            foreach (var path in editorcodes)
            {
                var guid = AssetDatabase.AssetPathToGUID(path);

                if (referenceCollection.Exists(c => c.fileGuid == guid) == false) continue;

                var referenceGuids = referenceCollection.First(c => c.fileGuid == guid).referenceGids;


                if (referenceGuids.Any(c => deleteFileList.Contains(c)) == false)
                    UnregistFromDelteList(AssetDatabase.AssetPathToGUID(path));
            }
        }

        private void UnregistFromDelteList(string guid)
        {
            if (deleteFileList.Contains(guid) == false) return;
            deleteFileList.Remove(guid);

            if (referenceCollection.Exists(c => c.fileGuid == guid))
            {
                var refInfo = referenceCollection.First(c => c.fileGuid == guid);
                foreach (var referenceGuid in refInfo.referenceGids) UnregistFromDelteList(referenceGuid);
            }
        }
    }
}