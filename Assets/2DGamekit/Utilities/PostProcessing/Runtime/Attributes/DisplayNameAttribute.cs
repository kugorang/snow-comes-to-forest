#region

using System;

#endregion

namespace UnityEngine.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DisplayNameAttribute : Attribute
    {
        public readonly string displayName;

        public DisplayNameAttribute(string displayName)
        {
            this.displayName = displayName;
        }
    }
}