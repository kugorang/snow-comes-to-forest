#region

using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    /// <summary>
    ///     Helper for drawing embedded asset editors
    /// </summary>
    public class EmbeddeAssetEditor<T> where T : ScriptableObject
    {
        public delegate void OnChangedDelegate(T obj);

        public delegate void OnCreateEditorDelegate(UnityEditor.Editor editor);

        private const int kIndentOffset = 6;

        private readonly bool m_DoVersionControlChecks;

        private readonly string m_PropertyName;

        /// <summary>
        ///     Customize this after creation if you want
        /// </summary>
        public GUIContent m_CreateButtonGUIContent;

        private UnityEditor.Editor m_Editor;
        private UnityEditor.Editor m_Owner;

        /// <summary>
        ///     Called when the asset being edited was changed by the user.
        /// </summary>
        public OnChangedDelegate OnChanged;

        /// <summary>
        ///     Called after the asset editor is created, in case it needs
        ///     to be customized
        /// </summary>
        public OnCreateEditorDelegate OnCreateEditor;

        /// <summary>
        ///     Create in OnEnable()
        /// </summary>
        public EmbeddeAssetEditor(string propertyName, UnityEditor.Editor owner)
        {
            m_PropertyName = propertyName;
            m_Owner = owner;
            m_DoVersionControlChecks = Provider.isActive;
            m_CreateButtonGUIContent = new GUIContent(
                "Create Asset", "Create a new shared settings asset");
        }

        /// <summary>
        ///     Free the resources in OnDisable()
        /// </summary>
        public void OnDisable()
        {
            DestroyEditor();
            m_Owner = null;
        }

        /// <summary>
        ///     Call this from OnInspectorGUI.  Will draw the asset reference field, and
        ///     the embedded editor, or a Create Asset button, if no asset is set.
        /// </summary>
        public void DrawEditorCombo(
            string title, string defaultName, string extension, string message,
            string showLabel, bool indent)
        {
            var property = m_Owner.serializedObject.FindProperty(m_PropertyName);
            if (m_Editor == null)
                UpdateEditor();
            if (m_Editor == null)
            {
                AssetFieldWithCreateButton(property, title, defaultName, extension, message);
            }
            else
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                var rect = EditorGUILayout.GetControlRect(true);
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, property);
                if (EditorGUI.EndChangeCheck())
                {
                    m_Owner.serializedObject.ApplyModifiedProperties();
                    UpdateEditor();
                }

                if (m_Editor != null)
                {
                    var foldoutRect = new Rect(
                        rect.x - kIndentOffset, rect.y, rect.width + kIndentOffset, rect.height);
                    property.isExpanded = EditorGUI.Foldout(
                        foldoutRect, property.isExpanded, GUIContent.none);

                    var targetAsset
                        = Provider.GetAssetByPath(
                            AssetDatabase.GetAssetPath(m_Editor.target));
                    var isLockedFile = m_DoVersionControlChecks
                                       && !targetAsset.IsOneOfStates(new[]
                                       {
                                           Asset.States.CheckedOutLocal,
                                           Asset.States.AddedLocal
                                       });

                    GUI.enabled = !isLockedFile;
                    if (property.isExpanded)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox(
                            "This is a shared asset.  Changes made here will apply to all users of this asset.",
                            MessageType.Info);
                        EditorGUI.BeginChangeCheck();
                        if (indent)
                            ++EditorGUI.indentLevel;
                        m_Editor.OnInspectorGUI();
                        if (indent)
                            --EditorGUI.indentLevel;
                        if (EditorGUI.EndChangeCheck() && OnChanged != null)
                            OnChanged(property.objectReferenceValue as T);
                    }

                    GUI.enabled = true;
                    if (isLockedFile && GUILayout.Button("Check out"))
                        Provider.Checkout(
                            targetAsset, CheckoutMode.Both);
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void AssetFieldWithCreateButton(
            SerializedProperty property,
            string title, string defaultName, string extension, string message)
        {
            EditorGUI.BeginChangeCheck();

            float hSpace = 5;
            var buttonWidth = GUI.skin.button.CalcSize(m_CreateButtonGUIContent).x;
            var r = EditorGUILayout.GetControlRect(true);
            r.width -= buttonWidth + hSpace;
            EditorGUI.PropertyField(r, property);
            r.x += r.width + hSpace;
            r.width = buttonWidth;
            if (GUI.Button(r, m_CreateButtonGUIContent))
            {
                var newAssetPath = EditorUtility.SaveFilePanelInProject(
                    title, defaultName, extension, message);
                if (!string.IsNullOrEmpty(newAssetPath))
                {
                    var asset = ScriptableObjectUtility.CreateAt<T>(newAssetPath);
                    property.objectReferenceValue = asset;
                    m_Owner.serializedObject.ApplyModifiedProperties();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                m_Owner.serializedObject.ApplyModifiedProperties();
                UpdateEditor();
            }
        }

        public void DestroyEditor()
        {
            if (m_Editor != null)
            {
                Object.DestroyImmediate(m_Editor);
                m_Editor = null;
            }
        }

        public void UpdateEditor()
        {
            DestroyEditor();
            var property = m_Owner.serializedObject.FindProperty(m_PropertyName);
            if (property.objectReferenceValue != null)
            {
                m_Editor = UnityEditor.Editor.CreateEditor(property.objectReferenceValue);
                if (OnCreateEditor != null)
                    OnCreateEditor(m_Editor);
            }
        }
    }
}