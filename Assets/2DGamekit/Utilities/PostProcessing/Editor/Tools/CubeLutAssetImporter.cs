#region

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    internal sealed class CubeLutAssetImporter : AssetPostprocessor
    {
        private static readonly List<string> s_Excluded = new List<string>
        {
            "Linear to sRGB r1",
            "Linear to Unity Log r1",
            "sRGB to Linear r1",
            "sRGB to Unity Log r1",
            "Unity Log to Linear r1",
            "Unity Log to sRGB r1"
        };

        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved,
            string[] movedFrom)
        {
            foreach (var path in imported)
            {
                var ext = Path.GetExtension(path);
                var filename = Path.GetFileNameWithoutExtension(path);

                if (string.IsNullOrEmpty(ext) || s_Excluded.Contains(filename))
                    continue;

                ext = ext.ToLowerInvariant();
                if (ext.Equals(".cube"))
                    ImportCubeLut(path);
            }
        }

        // Basic CUBE lut parser
        // Specs: http://wwwimages.adobe.com/content/dam/Adobe/en/products/speedgrade/cc/pdfs/cube-lut-specification-1.0.pdf
        private static void ImportCubeLut(string path)
        {
            // Remove the 'Assets' part of the path & build absolute path
            var fullpath = path.Substring(7);
            fullpath = Path.Combine(Application.dataPath, fullpath);

            // Read the lut data
            var lines = File.ReadAllLines(fullpath);

            // Start parsing
            var i = 0;
            var size = -1;
            var sizeCube = -1;
            var table = new List<Color>();
            var domainMin = Color.black;
            var domainMax = Color.white;

            while (true)
            {
                if (i >= lines.Length)
                {
                    if (table.Count != sizeCube)
                        Debug.LogError("Premature end of file");

                    break;
                }

                var line = FilterLine(lines[i]);

                if (string.IsNullOrEmpty(line))
                    goto next;

                // Header data
                if (line.StartsWith("TITLE"))
                    goto next; // Skip the title tag, we don't need it

                if (line.StartsWith("LUT_3D_SIZE"))
                {
                    var sizeStr = line.Substring(11).TrimStart();

                    if (!int.TryParse(sizeStr, out size))
                    {
                        Debug.LogError("Invalid data on line " + i);
                        break;
                    }

                    if (size < 2 || size > 256)
                    {
                        Debug.LogError("LUT size out of range");
                        break;
                    }

                    sizeCube = size * size * size;
                    goto next;
                }

                if (line.StartsWith("DOMAIN_MIN"))
                {
                    if (!ParseDomain(i, line, ref domainMin)) break;
                    goto next;
                }

                if (line.StartsWith("DOMAIN_MAX"))
                {
                    if (!ParseDomain(i, line, ref domainMax)) break;
                    goto next;
                }

                // Table
                var row = line.Split();

                if (row.Length != 3)
                {
                    Debug.LogError("Invalid data on line " + i);
                    break;
                }

                var color = Color.black;
                for (var j = 0; j < 3; j++)
                {
                    float d;
                    if (!float.TryParse(row[j], out d))
                    {
                        Debug.LogError("Invalid data on line " + i);
                        break;
                    }

                    color[j] = d;
                }

                table.Add(color);

                next:
                i++;
            }

            if (sizeCube != table.Count)
            {
                Debug.LogError("Wrong table size - Expected " + sizeCube + " elements, got " + table.Count);
                return;
            }

            // Check if the Texture3D already exists, update it in this case (better workflow for
            // the user)
            var assetPath = Path.ChangeExtension(path, ".asset");
            var tex = AssetDatabase.LoadAssetAtPath<Texture3D>(assetPath);

            if (tex != null)
            {
                tex.SetPixels(table.ToArray(), 0);
                tex.Apply();
            }
            else
            {
                // Generate a new Texture3D
                tex = new Texture3D(size, size, size, TextureFormat.RGBAHalf, false)
                {
                    anisoLevel = 0,
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                };

                tex.SetPixels(table.ToArray(), 0);
                tex.Apply();

                // Save to disk
                AssetDatabase.CreateAsset(tex, assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string FilterLine(string line)
        {
            var filtered = new StringBuilder();
            line = line.TrimStart().TrimEnd();
            var len = line.Length;
            var i = 0;

            while (i < len)
            {
                var c = line[i];

                if (c == '#') // Filters comment out
                    break;

                filtered.Append(c);
                i++;
            }

            return filtered.ToString();
        }

        private static bool ParseDomain(int i, string line, ref Color domain)
        {
            var domainStrs = line.Substring(10).TrimStart().Split();

            if (domainStrs.Length != 3)
            {
                Debug.LogError("Invalid data on line " + i);
                return false;
            }

            for (var j = 0; j < 3; j++)
            {
                float d;
                if (!float.TryParse(domainStrs[j], out d))
                {
                    Debug.LogError("Invalid data on line " + i);
                    return false;
                }

                domain[j] = d;
            }

            return true;
        }
    }
}