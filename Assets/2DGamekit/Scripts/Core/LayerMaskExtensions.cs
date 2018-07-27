#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public static class LayerMaskExtensions
    {
        public static bool Contains(this LayerMask layers, GameObject gameObject)
        {
            return 0 != (layers.value & (1 << gameObject.layer));
        }
    }
}