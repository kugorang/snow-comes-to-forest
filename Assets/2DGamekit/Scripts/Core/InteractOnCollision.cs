#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(Collider))]
    public class InteractOnCollision : MonoBehaviour
    {
        public LayerMask layers;
        public UnityEvent OnCollision;

        private void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
        }

        private void OnCollisionEnter(Collision c)
        {
            if (0 != (layers.value & (1 << c.transform.gameObject.layer))) OnCollision.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InteractionTrigger", false);
        }
    }
}