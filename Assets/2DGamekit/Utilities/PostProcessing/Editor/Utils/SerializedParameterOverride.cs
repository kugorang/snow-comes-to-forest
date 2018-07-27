#region

using System;
using System.Linq;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    public sealed class SerializedParameterOverride
    {
        internal SerializedProperty baseProperty;

        internal SerializedParameterOverride(SerializedProperty property, Attribute[] attributes)
        {
            baseProperty = property.Copy();

            var localCopy = baseProperty.Copy();
            localCopy.Next(true);
            overrideState = localCopy.Copy();
            localCopy.Next(false);
            value = localCopy.Copy();

            this.attributes = attributes;
        }

        public SerializedProperty overrideState { get; private set; }
        public SerializedProperty value { get; private set; }
        public Attribute[] attributes { get; private set; }

        public string displayName
        {
            get { return baseProperty.displayName; }
        }

        public T GetAttribute<T>()
            where T : Attribute
        {
            return (T) attributes.FirstOrDefault(x => x is T);
        }
    }
}