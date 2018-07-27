#region

using System;

#endregion

namespace UnityEngine.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MinMaxAttribute : Attribute
    {
        public readonly float max;
        public readonly float min;

        public MinMaxAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}