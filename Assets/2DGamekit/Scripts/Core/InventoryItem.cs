#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class InventoryItem : MonoBehaviour, IDataPersister
    {
        public AudioClip clip;

        [HideInInspector] public new CircleCollider2D collider;

        public DataSettings dataSettings;
        public bool disableOnEnter;
        public string inventoryKey = "";
        public LayerMask layers;

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<bool>(gameObject.activeSelf);
        }

        public void LoadData(Data data)
        {
            var inventoryItemData = (Data<bool>) data;
            gameObject.SetActive(inventoryItemData.value);
        }

        private void OnEnable()
        {
            collider = GetComponent<CircleCollider2D>();
            PersistentDataManager.RegisterPersister(this);
        }

        private void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        private void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
            collider = GetComponent<CircleCollider2D>();
            collider.radius = 5;
            collider.isTrigger = true;
            dataSettings = new DataSettings();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (layers.Contains(other.gameObject))
            {
                var ic = other.GetComponent<InventoryController>();
                ic.AddItem(inventoryKey);
                if (disableOnEnter)
                {
                    gameObject.SetActive(false);
                    Save();
                }

                if (clip) AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }

        public void Save()
        {
            PersistentDataManager.SetDirty(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InventoryItem", false);
        }
    }
}