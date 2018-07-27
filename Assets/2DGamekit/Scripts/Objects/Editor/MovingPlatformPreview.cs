#region

using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class MovingPlatformPreview
    {
        public static MovingPlatformPreview s_Preview = null;
        public static GameObject preview;

        protected static MovingPlatform movingPlatform;

        static MovingPlatformPreview()
        {
            Selection.selectionChanged += SelectionChanged;
        }

        private static void SelectionChanged()
        {
            if (movingPlatform != null && Selection.activeGameObject != movingPlatform.gameObject) DestroyPreview();
        }

        public static void DestroyPreview()
        {
            if (preview == null)
                return;

            Object.DestroyImmediate(preview);
            preview = null;
            movingPlatform = null;
        }

        public static void CreateNewPreview(MovingPlatform origin)
        {
            if (preview != null) Object.DestroyImmediate(preview);

            movingPlatform = origin;

            preview = Object.Instantiate(origin.gameObject);
            preview.hideFlags = HideFlags.DontSave;
            var plt = preview.GetComponentInChildren<MovingPlatform>();
            Object.DestroyImmediate(plt);


            var c = new Color(0.2f, 0.2f, 0.2f, 0.4f);
            var rends = preview.GetComponentsInChildren<SpriteRenderer>();
            for (var i = 0; i < rends.Length; ++i)
                rends[i].color = c;
        }
    }
}