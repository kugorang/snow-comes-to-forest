#region

using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomPropertyDrawer(typeof(ScrollingTextBehaviour))]
    public class ScrollingTextDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var fieldCount = 3;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var messageProp = property.FindPropertyRelative("message");
            var startDelayProp = property.FindPropertyRelative("startDelay");
            var holdDelayProp = property.FindPropertyRelative("holdDelay");

            var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, messageProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, startDelayProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, holdDelayProp);
        }
    }
}