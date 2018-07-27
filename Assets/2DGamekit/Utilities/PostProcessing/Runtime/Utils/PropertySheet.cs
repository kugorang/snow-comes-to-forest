namespace UnityEngine.Rendering.PostProcessing
{
    public sealed class PropertySheet
    {
        internal PropertySheet(Material material)
        {
            this.material = material;
            properties = new MaterialPropertyBlock();
        }

        public MaterialPropertyBlock properties { get; private set; }
        internal Material material { get; private set; }

        public void ClearKeywords()
        {
            material.shaderKeywords = null;
        }

        public void EnableKeyword(string keyword)
        {
            material.EnableKeyword(keyword);
        }

        public void DisableKeyword(string keyword)
        {
            material.DisableKeyword(keyword);
        }

        internal void Release()
        {
            RuntimeUtilities.Destroy(material);
            material = null;
        }
    }
}