#region

using UnityEditor;

#endregion

namespace Gamekit2D
{
    [CustomEditor(typeof(PressurePad))]
    public class PressurePadEditor : Editor
    {
        private SerializedProperty m_ActivatedBoxSpriteProp;
        private SerializedProperty m_ActivationTypeProp;
        private SerializedProperty m_BoxesProp;
        private SerializedProperty m_DeactivatedBoxSpriteProp;
        private SerializedProperty m_OnPressedProp;
        private SerializedProperty m_OnReleaseProp;
        private SerializedProperty m_PlatformCatcherProp;
        private SerializedProperty m_RequiredCountProp;
        private SerializedProperty m_RequiredMassProp;

        private void OnEnable()
        {
            m_PlatformCatcherProp = serializedObject.FindProperty("platformCatcher");
            m_ActivationTypeProp = serializedObject.FindProperty("activationType");
            m_RequiredCountProp = serializedObject.FindProperty("requiredCount");
            m_RequiredMassProp = serializedObject.FindProperty("requiredMass");
            m_DeactivatedBoxSpriteProp = serializedObject.FindProperty("deactivatedBoxSprite");
            m_ActivatedBoxSpriteProp = serializedObject.FindProperty("activatedBoxSprite");
            m_BoxesProp = serializedObject.FindProperty("boxes");
            m_OnPressedProp = serializedObject.FindProperty("OnPressed");
            m_OnReleaseProp = serializedObject.FindProperty("OnRelease");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_PlatformCatcherProp);
            EditorGUILayout.PropertyField(m_ActivationTypeProp);
            if ((PressurePad.ActivationType) m_ActivationTypeProp.enumValueIndex ==
                PressurePad.ActivationType.ItemCount)
                EditorGUILayout.PropertyField(m_RequiredCountProp);
            else
                EditorGUILayout.PropertyField(m_RequiredMassProp);

            EditorGUILayout.PropertyField(m_DeactivatedBoxSpriteProp);
            EditorGUILayout.PropertyField(m_ActivatedBoxSpriteProp);
            EditorGUILayout.PropertyField(m_BoxesProp, true);


            EditorGUILayout.PropertyField(m_OnPressedProp);
            EditorGUILayout.PropertyField(m_OnReleaseProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}