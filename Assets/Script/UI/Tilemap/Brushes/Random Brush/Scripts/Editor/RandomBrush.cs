#region

using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

#endregion

namespace UnityEditor
{
    [CustomGridBrush(false, true, false, "Random Brush")]
    public class RandomBrush : GridBrush
    {
        public TileBase[] randomTiles;

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (randomTiles != null && randomTiles.Length > 0)
            {
                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                var min = position - pivot;
                var bounds = new BoundsInt(min, size);
                foreach (var location in bounds.allPositionsWithin)
                {
                    var randomTile = randomTiles[(int) (randomTiles.Length * Random.value)];
                    tilemap.SetTile(location, randomTile);
                }
            }
            else
            {
                base.Paint(grid, brushTarget, position);
            }
        }

        [MenuItem("Assets/Create/Random Brush")]
        public static void CreateBrush()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Random Brush", "New Random Brush", "asset",
                "Save Random Brush", "Assets");

            if (path == "")
                return;

            AssetDatabase.CreateAsset(CreateInstance<RandomBrush>(), path);
        }
    }

    [CustomEditor(typeof(RandomBrush))]
    public class RandomBrushEditor : GridBrushEditor
    {
        private GameObject lastBrushTarget;

        private RandomBrush randomBrush
        {
            get { return target as RandomBrush; }
        }

        public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (randomBrush.randomTiles != null && randomBrush.randomTiles.Length > 0)
            {
                base.PaintPreview(grid, null, position);

                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                var min = position - randomBrush.pivot;
                var bounds = new BoundsInt(min, randomBrush.size);
                foreach (var location in bounds.allPositionsWithin)
                {
                    var randomTile = randomBrush.randomTiles[(int) (randomBrush.randomTiles.Length * Random.value)];
                    tilemap.SetEditorPreviewTile(location, randomTile);
                }

                lastBrushTarget = brushTarget;
            }
            else
            {
                base.PaintPreview(grid, brushTarget, position);
            }
        }

        public override void ClearPreview()
        {
            if (lastBrushTarget != null)
            {
                var tilemap = lastBrushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                tilemap.ClearAllEditorPreviewTiles();

                lastBrushTarget = null;
            }
            else
            {
                base.ClearPreview();
            }
        }

        public override void OnPaintInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            var count = EditorGUILayout.IntField("Number of Tiles",
                randomBrush.randomTiles != null ? randomBrush.randomTiles.Length : 0);
            if (count < 0)
                count = 0;
            if (randomBrush.randomTiles == null || randomBrush.randomTiles.Length != count)
                Array.Resize(ref randomBrush.randomTiles, count);

            if (count == 0)
                return;

            EditorGUILayout.LabelField("Place random tiles.");
            EditorGUILayout.Space();

            for (var i = 0; i < count; i++)
                randomBrush.randomTiles[i] = (TileBase) EditorGUILayout.ObjectField("Tile " + (i + 1),
                    randomBrush.randomTiles[i], typeof(TileBase), false, null);
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(randomBrush);
        }
    }
}