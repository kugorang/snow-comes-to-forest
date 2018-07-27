#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class GunnerBeam : MonoBehaviour
    {
        public Transform target;

        private void Update()
        {
            transform.LookAt(target);
        }
    }
}