#region

using System;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class BossDoorStateManager : MonoBehaviour
    {
        public DoorState[] doorStates;


        public InventoryController inventoryController;

        protected int m_Keys;
        public SpriteRenderer spriteRenderer;

        private void Start()
        {
            UpdateStates();
        }

        public void UpdateStates()
        {
            for (var i = 0; i < doorStates.Length; i++) doorStates[i].UpdateState(inventoryController, spriteRenderer);
        }

        [Serializable]
        public class DoorState
        {
            public string keyInventoryName;
            public Light light;
            public Sprite sprite;

            public void UpdateState(InventoryController inventoryController, SpriteRenderer spriteRenderer)
            {
                var hasKey = inventoryController.HasItem(keyInventoryName);
                if (hasKey)
                    spriteRenderer.sprite = sprite;
                light.enabled = hasKey;
            }
        }
    }
}