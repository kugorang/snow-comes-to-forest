#region

using System;

#endregion

namespace UnityEngine.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MinAttribute : Attribute
    {
        public readonly float min;

        public MinAttribute(float min)
        {
            this.min = min;
        }
    }
}