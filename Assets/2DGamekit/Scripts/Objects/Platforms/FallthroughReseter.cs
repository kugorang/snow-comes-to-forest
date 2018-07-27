#region

using System.Collections;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class FallthroughReseter : MonoBehaviour
    {
        public void StartFall(PlatformEffector2D effector)
        {
            StartCoroutine(FallCoroutine(effector));
        }

        private IEnumerator FallCoroutine(PlatformEffector2D effector)
        {
            var playerLayerMask = 1 << LayerMask.NameToLayer("Player");

            effector.colliderMask &= ~playerLayerMask;
            gameObject.layer = LayerMask.NameToLayer("Default");

            yield return new WaitForSeconds(0.5f);

            effector.colliderMask |= playerLayerMask;
            gameObject.layer = LayerMask.NameToLayer("Platform");

            Destroy(this);
        }
    }
}