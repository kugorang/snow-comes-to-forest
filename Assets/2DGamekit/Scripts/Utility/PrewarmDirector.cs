#region

using UnityEngine;
using UnityEngine.Playables;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PrewarmDirector : MonoBehaviour
    {
        private void OnEnable()
        {
            GetComponent<PlayableDirector>().RebuildGraph();
        }
    }
}