#region

using System;

#endregion

namespace UnityEngine.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MaxAttribute : Attribute
    {
        public readonly float max;

        public MaxAttribute(float max)
        {
            this.max = max;
        }
    }
}