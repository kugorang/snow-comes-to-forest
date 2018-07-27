#region

using System;

#endregion

namespace UnityEngine.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class TrackballAttribute : Attribute
    {
        public enum Mode
        {
            None,
            Lift,
            Gamma,
            Gain
        }

        public readonly Mode mode;

        public TrackballAttribute(Mode mode)
        {
            this.mode = mode;
        }
    }
}