#region

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace SaveDuringPlay
{
    /// <summary>A collection of tools for finding objects</summary>
    public static class ObjectTreeUtil
    {
        /// <summary>
        ///     Get the full name of an object, travelling up the transform parents to the root.
        /// </summary>
        public static string GetFullName(GameObject current)
        {
            if (current == null)
                return "";
            if (current.transform.parent == null)
                return "/" + current.name;
            return GetFullName(current.transform.parent.gameObject) + "/" + current.name;
        }

        /// <summary>
        ///     Will find the named object, active or inactive, from the full path.
        /// </summary>
        public static GameObject FindObjectFromFullName(string fullName, GameObject[] roots)
        {
            if (fullName == null || fullName.Length == 0 || roots == null)
                return null;

            var path = fullName.Split('/');
            if (path.Length < 2) // skip leading '/'
                return null;

            Transform root = null;
            for (var i = 0; root == null && i < roots.Length; ++i)
                if (roots[i].name == path[1])
                    root = roots[i].transform;

            if (root == null)
                return null;

            for (var i = 2; i < path.Length; ++i) // skip root
            {
                var found = false;
                for (var c = 0; c < root.childCount; ++c)
                {
                    var child = root.GetChild(c);
                    if (child.name == path[i])
                    {
                        found = true;
                        root = child;
                        break;
                    }
                }

                if (!found)
                    return null;
            }

            return root.gameObject;
        }

        /// <summary>Finds all the root objects in a scene, active or not</summary>
        public static GameObject[] FindAllRootObjectsInScene()
        {
            return SceneManager.GetActiveScene().GetRootGameObjects();
        }


        /// <summary>
        ///     This finds all the behaviours in scene, active or inactive, excluding prefabs
        /// </summary>
        public static T[] FindAllBehavioursInScene<T>() where T : MonoBehaviour
        {
            var objectsInScene = new List<T>();
            foreach (var b in Resources.FindObjectsOfTypeAll<T>())
            {
                var go = b.gameObject;
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                    continue;
                if (EditorUtility.IsPersistent(go.transform.root.gameObject))
                    continue;
                objectsInScene.Add(b);
            }

            return objectsInScene.ToArray();
        }
    }

    internal class GameObjectFieldScanner
    {
        public delegate bool FilterFieldDelegate(string fullName, FieldInfo fieldInfo);

        public delegate bool OnFieldValueChangedDelegate(
            string fullName, FieldInfo fieldInfo, object fieldOwner, object value);

        public delegate bool OnLeafFieldDelegate(string fullName, Type type, ref object value);

        /// <summary>
        ///     Which fields will be scanned
        /// </summary>
        public BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        ///     Called for each field, to test whether to proceed with scanning it.  Return true to scan.
        /// </summary>
        public FilterFieldDelegate FilterField;

        /// <summary>
        ///     Called for each field node, if and only if OnLeafField() for it or one
        ///     of its leaves returned true.
        /// </summary>
        public OnFieldValueChangedDelegate OnFieldValueChanged;

        /// <summary>
        ///     Called for each leaf field.  Return value should be true if action was taken.
        ///     It will be propagated back to the caller.
        /// </summary>
        public OnLeafFieldDelegate OnLeafField;

        private bool ScanFields(string fullName, Type type, ref object obj)
        {
            var doneSomething = false;

            // Check if it's a complex type
            var isLeaf = true;
            if (obj != null
                && !type.IsSubclassOf(typeof(Component))
                && !type.IsSubclassOf(typeof(GameObject)))
            {
                // Is it an array?
                if (type.IsArray)
                {
                    isLeaf = false;
                    var array = obj as Array;
                    object arrayLength = array.Length;
                    if (OnLeafField != null && OnLeafField(
                            fullName + ".Length", arrayLength.GetType(), ref arrayLength))
                    {
                        var newArray = Array.CreateInstance(
                            array.GetType().GetElementType(), Convert.ToInt32(arrayLength));
                        Array.Copy(array, 0, newArray, 0, Math.Min(array.Length, newArray.Length));
                        array = newArray;
                        doneSomething = true;
                    }

                    for (var i = 0; i < array.Length; ++i)
                    {
                        var element = array.GetValue(i);
                        if (ScanFields(fullName + "[" + i + "]", array.GetType().GetElementType(), ref element))
                        {
                            array.SetValue(element, i);
                            doneSomething = true;
                        }
                    }

                    if (doneSomething)
                        obj = array;
                }
                else
                {
                    // Check if it's a complex type
                    var fields = obj.GetType().GetFields(bindingFlags);
                    if (fields.Length > 0)
                    {
                        isLeaf = false;
                        for (var i = 0; i < fields.Length; ++i)
                        {
                            var name = fullName + "." + fields[i].Name;
                            if (FilterField == null || FilterField(name, fields[i]))
                            {
                                var fieldValue = fields[i].GetValue(obj);
                                if (ScanFields(name, fields[i].FieldType, ref fieldValue))
                                {
                                    doneSomething = true;
                                    if (OnFieldValueChanged != null)
                                        OnFieldValueChanged(name, fields[i], obj, fieldValue);
                                }
                            }
                        }
                    }
                }
            }

            // If it's a leaf field then call the leaf handler
            if (isLeaf && OnLeafField != null)
                if (OnLeafField(fullName, type, ref obj))
                    doneSomething = true;

            return doneSomething;
        }

        public bool ScanFields(string fullName, MonoBehaviour b)
        {
            var doneSomething = false;
            var fields = b.GetType().GetFields(bindingFlags);
            if (fields.Length > 0)
                for (var i = 0; i < fields.Length; ++i)
                {
                    var name = fullName + "." + fields[i].Name;
                    if (FilterField == null || FilterField(name, fields[i]))
                    {
                        var fieldValue = fields[i].GetValue(b);
                        if (ScanFields(name, fields[i].FieldType, ref fieldValue))
                            doneSomething = true;

                        // If leaf action was taken, propagate it up to the parent node
                        if (doneSomething && OnFieldValueChanged != null)
                            OnFieldValueChanged(fullName, fields[i], b, fieldValue);
                    }
                }

            return doneSomething;
        }

        /// <summary>
        ///     Recursively scan the MonoBehaviours of a GameObject and its children.
        ///     For each leaf field found, call the OnFieldValue delegate.
        /// </summary>
        public bool ScanFields(GameObject go, string prefix = null)
        {
            var doneSomething = false;
            if (prefix == null)
                prefix = "";
            else if (prefix.Length > 0)
                prefix += ".";

            var components = go.GetComponents<MonoBehaviour>();
            for (var i = 0; i < components.Length; ++i)
            {
                var c = components[i];
                if (c != null && ScanFields(prefix + c.GetType().FullName + i, c))
                    doneSomething = true;
            }

            return doneSomething;
        }
    }


    /// <summary>
    ///     Using reflection, this class scans a GameObject (and optionally its children)
    ///     and records all the field settings.  This only works for "nice" field settings
    ///     within MonoBehaviours.  Changes to the behaviour stack made between saving
    ///     and restoring will fool this class.
    /// </summary>
    internal class ObjectStateSaver
    {
        private readonly Dictionary<string, string> mValues = new Dictionary<string, string>();
        public string ObjetFullPath { get; private set; }

        /// <summary>
        ///     Recursively collect all the field values in the MonoBehaviours
        ///     owned by this object and its descendants.  The values are stored
        ///     in an internal dictionary.
        /// </summary>
        public void CollectFieldValues(GameObject go)
        {
            ObjetFullPath = ObjectTreeUtil.GetFullName(go);
            var scanner = new GameObjectFieldScanner();
            scanner.FilterField = FilterField;
            scanner.OnLeafField = (string fullName, Type type, ref object value) =>
            {
                // Save the value in the dictionary
                mValues[fullName] = StringFromLeafObject(value);
                //Debug.Log(mObjectFullPath + "." + fullName + " = " + mValues[fullName]);
                return false;
            };
            scanner.ScanFields(go);
        }

        public GameObject FindSavedGameObject(GameObject[] roots)
        {
            return ObjectTreeUtil.FindObjectFromFullName(ObjetFullPath, roots);
        }

        /// <summary>
        ///     Recursively scan the MonoBehaviours of a GameObject and its children.
        ///     For each field found, look up its value in the internal dictionary.
        ///     If it's present and its value in the dictionary differs from the actual
        ///     value in the game object, Set the GameObject's value using the value
        ///     recorded in the dictionary.
        /// </summary>
        public bool PutFieldValues(GameObject go, GameObject[] roots)
        {
            var scanner = new GameObjectFieldScanner();
            scanner.FilterField = FilterField;
            scanner.OnLeafField = (string fullName, Type type, ref object value) =>
            {
                // Lookup the value in the dictionary
                string savedValue;
                if (mValues.TryGetValue(fullName, out savedValue)
                    && StringFromLeafObject(value) != savedValue)
                {
                    //Debug.Log(mObjectFullPath + "." + fullName + " = " + mValues[fullName]);
                    value = LeafObjectFromString(type, mValues[fullName].Trim(), roots);
                    return true; // changed
                }

                return false;
            };
            scanner.OnFieldValueChanged = (fullName, fieldInfo, fieldOwner, value) =>
            {
                fieldInfo.SetValue(fieldOwner, value);
                return true;
            };
            return scanner.ScanFields(go);
        }

        /// Ignore fields marked with the [NoSaveDuringPlay] attribute
        private bool FilterField(string fullName, FieldInfo fieldInfo)
        {
            var attrs = fieldInfo.GetCustomAttributes(false);
            foreach (var attr in attrs)
                if (attr.GetType().Name.Contains("NoSaveDuringPlay"))
                    return false;
            return true;
        }

        /// <summary>
        ///     Parse a string to generate an object.
        ///     Only very limited primitive object types are supported.
        ///     Enums, Vectors and most other structures are automatically supported,
        ///     because the reflection system breaks them down into their primitive components.
        ///     You can add more support here, as needed.
        /// </summary>
        private static object LeafObjectFromString(Type type, string value, GameObject[] roots)
        {
            if (type == typeof(float))
                return float.Parse(value);
            if (type == typeof(double))
                return double.Parse(value);
            if (type == typeof(bool))
                return bool.Parse(value);
            if (type == typeof(string))
                return value;
            if (type == typeof(int))
                return int.Parse(value);
            if (type == typeof(uint))
                return uint.Parse(value);
            if (type.IsSubclassOf(typeof(Component)))
            {
                // Try to find the named game object
                var go = ObjectTreeUtil.FindObjectFromFullName(value, roots);
                return go != null ? go.GetComponent(type) : null;
            }

            if (type.IsSubclassOf(typeof(GameObject))) return GameObject.Find(value);
            return null;
        }

        private static string StringFromLeafObject(object obj)
        {
            if (obj == null)
                return string.Empty;

            if (obj.GetType().IsSubclassOf(typeof(Component)))
            {
                var c = (Component) obj;
                if (c == null) // Component overrides the == operator, so we have to check
                    return string.Empty;
                return ObjectTreeUtil.GetFullName(c.gameObject);
            }

            if (obj.GetType().IsSubclassOf(typeof(GameObject)))
            {
                var go = (GameObject) obj;
                if (go == null) // GameObject overrides the == operator, so we have to check
                    return string.Empty;
                return ObjectTreeUtil.GetFullName(go);
            }

            return obj.ToString();
        }
    }


    /// <summary>
    ///     For all registered object types, record their state when exiting Play Mode,
    ///     and restore that state to the objects in the scene.  This is a very limited
    ///     implementation which has not been rigorously tested with many objects types.
    ///     It's quite possible that not everything will be saved.
    ///     This class is expected to become obsolete when Unity implements this functionality
    ///     in a more general way.
    ///     To use this class,
    ///     drop this script into your project, and add the [SaveDuringPlay] attribute to your class.
    ///     Note: if you want some specific field in your class NOT to be saved during play,
    ///     add a property attribute whose class name contains the string "NoSaveDuringPlay"
    ///     and the field will not be saved.
    /// </summary>
    [InitializeOnLoad]
    public class SaveDuringPlay
    {
        public static string kEnabledKey = "SaveDuringPlay_Enabled";

        public static bool Enabled
        {
            get { return EditorPrefs.GetBool(kEnabledKey, false); }
            set
            {
                if (value != Enabled) EditorPrefs.SetBool(kEnabledKey, value);
            }
        }

        static SaveDuringPlay()
        {
            // Install our callbacks
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlayStateChanged;
#else
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playmodeStateChanged += OnPlayStateChanged;
#endif
        }

#if UNITY_2017_2_OR_NEWER
        private static void OnPlayStateChanged(PlayModeStateChange pmsc)
        {
            if (Enabled)
            {
                // If exiting playmode, collect the state of all interesting objects
                if (pmsc == PlayModeStateChange.ExitingPlayMode)
                    SaveAllInterestingStates();
                else if (pmsc == PlayModeStateChange.EnteredEditMode && sSavedStates != null)
                    RestoreAllInterestingStates();
            }
        }
#else
        static void OnPlayStateChanged()
        {
            // If exiting playmode, collect the state of all interesting objects
            if (Enabled)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
                    SaveAllInterestingStates();
            }
        }

        static float sWaitStartTime = 0;
        static void OnEditorUpdate()
        {
            if (Enabled && sSavedStates != null && !Application.isPlaying)
            {
                // Wait a bit for things to settle before applying the saved state
                const float WaitTime = 1f; // GML todo: is there a better way to do this?
                float time = Time.realtimeSinceStartup;
                if (sWaitStartTime == 0)
                    sWaitStartTime = time;
                else if (time - sWaitStartTime > WaitTime)
                {
                    RestoreAllInterestingStates();
                    sWaitStartTime = 0;
                }
            }
        }
#endif

        /// <summary>
        ///     If you need to get notified before state is collected for hotsave, this is the place
        /// </summary>
        public static OnHotSaveDelegate OnHotSave;

        public delegate void OnHotSaveDelegate();

        /// Collect all relevant objects, active or not
        private static Transform[] FindInterestingObjects()
        {
            var objects = new List<Transform>();
            var everything = ObjectTreeUtil.FindAllBehavioursInScene<MonoBehaviour>();
            foreach (var b in everything)
            {
                var attrs = b.GetType().GetCustomAttributes(true);
                foreach (var attr in attrs)
                    if (attr.GetType().Name.Contains("SaveDuringPlay"))
                    {
                        //Debug.Log("Found " + ObjectTreeUtil.GetFullName(b.gameObject) + " for hot-save"); 
                        objects.Add(b.transform);
                        break;
                    }
            }

            return objects.ToArray();
        }

        private static List<ObjectStateSaver> sSavedStates;
        private static GameObject sSaveStatesGameObject;

        private static void SaveAllInterestingStates()
        {
            //Debug.Log("Exiting play mode: Saving state for all interesting objects");
            if (OnHotSave != null)
                OnHotSave();

            sSavedStates = new List<ObjectStateSaver>();
            var objects = FindInterestingObjects();
            foreach (var obj in objects)
            {
                var saver = new ObjectStateSaver();
                saver.CollectFieldValues(obj.gameObject);
                sSavedStates.Add(saver);
            }

            if (sSavedStates.Count == 0)
                sSavedStates = null;
        }

        private static void RestoreAllInterestingStates()
        {
            //Debug.Log("Updating state for all interesting objects");
            var dirty = false;
            var roots = ObjectTreeUtil.FindAllRootObjectsInScene();
            foreach (var saver in sSavedStates)
            {
                var go = saver.FindSavedGameObject(roots);
                if (go != null)
                {
                    Undo.RegisterFullObjectHierarchyUndo(go, "SaveDuringPlay");
                    if (saver.PutFieldValues(go, roots))
                    {
                        //Debug.Log("SaveDuringPlay: updated settings of " + saver.ObjetFullPath);
                        EditorUtility.SetDirty(go);
                        dirty = true;
                    }
                }
            }

            if (dirty)
                InternalEditorUtility.RepaintAllViews();
            sSavedStates = null;
        }
    }
}