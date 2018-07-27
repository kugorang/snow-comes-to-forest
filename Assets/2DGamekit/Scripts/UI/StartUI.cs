﻿#region

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

#endregion

namespace Gamekit2D
{
    public class StartUI : MonoBehaviour
    {
        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
		Application.Quit();
    #endif
        }
    }
}