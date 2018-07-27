#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(SphereCollider))]
    public class InteractOnTrigger : MonoBehaviour
    {
        public InventoryController.InventoryChecker[] inventoryChecks;
        public LayerMask layers;

        private SphereCollider m_Collider;
        public UnityEvent OnEnter, OnExit;

        private void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
            m_Collider = GetComponent<SphereCollider>();
            m_Collider.radius = 5;
            m_Collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (layers.Contains(other.gameObject)) ExecuteOnEnter(other);
        }

        protected virtual void ExecuteOnEnter(Collider other)
        {
            OnEnter.Invoke();
            for (var i = 0; i < inventoryChecks.Length; i++)
                inventoryChecks[i].CheckInventory(other.GetComponentInChildren<InventoryController>());
        }

        private void OnTriggerExit(Collider other)
        {
            if (layers.Contains(other.gameObject)) ExecuteOnExit(other);
        }

        protected virtual void ExecuteOnExit(Collider other)
        {
            OnExit.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InteractionTrigger", false);
        }
    }
}