#region

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

#endregion

namespace Gamekit2D
{
    [RequireComponent(typeof(Collider2D))]
    public class DirectorTrigger : MonoBehaviour, IDataPersister
    {
        public enum TriggerType
        {
            Once,
            Everytime
        }

        [HideInInspector] public DataSettings dataSettings;

        public PlayableDirector director;

        protected bool m_AlreadyTriggered;
        public UnityEvent OnDirectorFinish;
        public UnityEvent OnDirectorPlay;

        [Tooltip("This is the gameobject which will trigger the director to play.  For example, the player.")]
        public GameObject triggeringGameObject;

        public TriggerType triggerType;

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
            return new Data<bool>(m_AlreadyTriggered);
        }

        public void LoadData(Data data)
        {
            var directorTriggerData = (Data<bool>) data;
            m_AlreadyTriggered = directorTriggerData.value;
        }

        private void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        private void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject != triggeringGameObject)
                return;

            if (triggerType == TriggerType.Once && m_AlreadyTriggered)
                return;

            director.Play();
            m_AlreadyTriggered = true;
            OnDirectorPlay.Invoke();
            Invoke("FinishInvoke", (float) director.duration);
        }

        private void FinishInvoke()
        {
            OnDirectorFinish.Invoke();
        }

        public void OverrideAlreadyTriggered(bool alreadyTriggered)
        {
            m_AlreadyTriggered = alreadyTriggered;
        }
    }
}