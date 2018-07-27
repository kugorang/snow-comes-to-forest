#region

using System;

#endregion

namespace UnityEngine.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PostProcessAttribute : Attribute
    {
        public readonly bool allowInSceneView;
        internal readonly bool builtinEffect;
        public readonly PostProcessEvent eventType;
        public readonly string menuItem;
        public readonly Type renderer;

        public PostProcessAttribute(Type renderer, PostProcessEvent eventType, string menuItem,
            bool allowInSceneView = true)
        {
            this.renderer = renderer;
            this.eventType = eventType;
            this.menuItem = menuItem;
            this.allowInSceneView = allowInSceneView;
            builtinEffect = false;
        }

        internal PostProcessAttribute(Type renderer, string menuItem, bool allowInSceneView = true)
        {
            this.renderer = renderer;
            this.menuItem = menuItem;
            this.allowInSceneView = allowInSceneView;
            builtinEffect = true;
        }
    }
}