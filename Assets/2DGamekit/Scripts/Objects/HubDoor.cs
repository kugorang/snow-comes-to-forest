#region

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Gamekit2D
{
    public class HubDoor : MonoBehaviour, IDataPersister
    {
        public InventoryController characterInventory;
        public DataSettings dataSettings;

        public DirectorTrigger keyDirectorTrigger;

        private SpriteRenderer m_SpriteRenderer;
        public UnityEvent onUnlocked;
        public string[] requiredInventoryItemKeys;
        public Sprite[] unlockStateSprites;

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void LoadData(Data data)
        {
            var d = data as Data<Sprite>;
            m_SpriteRenderer.sprite = d.value;
        }

        public Data SaveData()
        {
            return new Data<Sprite>(m_SpriteRenderer.sprite);
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        [ContextMenu("Update State")]
        private void CheckInventory()
        {
            var stateIndex = -1;
            foreach (var i in requiredInventoryItemKeys)
                if (characterInventory.HasItem(i))
                    stateIndex++;
            if (stateIndex >= 0)
            {
                keyDirectorTrigger.OverrideAlreadyTriggered(true);
                m_SpriteRenderer.sprite = unlockStateSprites[stateIndex];
                if (stateIndex == requiredInventoryItemKeys.Length - 1) onUnlocked.Invoke();
            }
        }

        private void OnEnable()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            characterInventory.OnInventoryLoaded += CheckInventory;
        }

        private void Update()
        {
            CheckInventory();
        }
    }
}