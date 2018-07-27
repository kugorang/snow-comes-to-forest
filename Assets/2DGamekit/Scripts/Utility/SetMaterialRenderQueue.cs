#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class SetMaterialRenderQueue : MonoBehaviour
    {
        public Material material;
        public int queueOverrideValue;

        private void Start()
        {
            material.renderQueue = queueOverrideValue;
        }
    }
}