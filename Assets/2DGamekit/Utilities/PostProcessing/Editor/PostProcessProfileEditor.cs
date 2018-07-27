#region

using UnityEngine.Rendering.PostProcessing;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    [CustomEditor(typeof(PostProcessProfile))]
    internal sealed class PostProcessProfileEditor : Editor
    {
        private EffectListEditor m_EffectList;

        private void OnEnable()
        {
            m_EffectList = new EffectListEditor(this);
            m_EffectList.Init(target as PostProcessProfile, serializedObject);
        }

        private void OnDisable()
        {
            if (m_EffectList != null)
                m_EffectList.Clear();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            m_EffectList.OnGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}