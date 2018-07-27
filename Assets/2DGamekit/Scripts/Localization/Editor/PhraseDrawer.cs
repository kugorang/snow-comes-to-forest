#region

using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomPropertyDrawer(typeof(Phrase))]
    public class PhraseDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyProp = property.FindPropertyRelative("key");
            var valueProp = property.FindPropertyRelative("value");

            var propertyRect = position;
            propertyRect.width *= 0.25f;
            EditorGUI.PropertyField(propertyRect, keyProp, GUIContent.none);

            propertyRect.x += propertyRect.width;
            propertyRect.width *= 3f;
            EditorGUI.PropertyField(propertyRect, valueProp, GUIContent.none);
        }
    }
}