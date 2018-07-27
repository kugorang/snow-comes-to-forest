#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractOnTrigger2D : MonoBehaviour
    {
        public InventoryController.InventoryChecker[] inventoryChecks;
        public LayerMask layers;

        protected Collider2D m_Collider;
        public UnityEvent OnEnter, OnExit;

        private void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
            m_Collider = GetComponent<Collider2D>();
            m_Collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled)
                return;

            if (layers.Contains(other.gameObject)) ExecuteOnEnter(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!enabled)
                return;

            if (layers.Contains(other.gameObject)) ExecuteOnExit(other);
        }

        protected virtual void ExecuteOnEnter(Collider2D other)
        {
            OnEnter.Invoke();
            for (var i = 0; i < inventoryChecks.Length; i++)
                inventoryChecks[i].CheckInventory(other.GetComponentInChildren<InventoryController>());
        }

        protected virtual void ExecuteOnExit(Collider2D other)
        {
            OnExit.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InteractionTrigger", false);
        }
    }
}