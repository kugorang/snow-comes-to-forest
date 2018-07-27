#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class WeaponPickup : MonoBehaviour, IDataPersister
    {
        public ParticleSystem blueMotes;
        public DataSettings dataSettings;
        public InteractOnTrigger2D interactOnTrigger2D;
        public SpriteRenderer spriteRenderer;

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
            return new Data<Sprite, bool, bool>(spriteRenderer.sprite, blueMotes.isPlaying,
                interactOnTrigger2D.enabled);
        }

        public void LoadData(Data data)
        {
            var weaponPickupData = (Data<Sprite, bool, bool>) data;
            spriteRenderer.sprite = weaponPickupData.value0;
            if (!weaponPickupData.value1)
                blueMotes.Stop();
            if (!weaponPickupData.value2)
                interactOnTrigger2D.enabled = false;
        }

        private void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        private void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }
    }
}