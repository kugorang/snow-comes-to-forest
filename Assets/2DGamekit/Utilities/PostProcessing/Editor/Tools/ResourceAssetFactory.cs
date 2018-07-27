#region

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    internal static class ResourceAssetFactory
    {
#if POSTFX_DEBUG_MENUS
        [MenuItem("Tools/Post-processing/Create Resources Asset")]
#endif
        private static void CreateAsset()
        {
            var asset = ScriptableObject.CreateInstance<PostProcessResources>();
            AssetDatabase.CreateAsset(asset, "Assets/PostProcessResources.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}