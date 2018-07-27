#region

using System;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DecoratorAttribute : Attribute
    {
        public readonly Type attributeType;

        public DecoratorAttribute(Type attributeType)
        {
            this.attributeType = attributeType;
        }
    }
}