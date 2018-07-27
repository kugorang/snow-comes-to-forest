#region

using UnityEditor;

#endregion

namespace Gamekit2D
{
    public abstract class DataPersisterEditor : Editor
    {
        private IDataPersister m_DataPersister;

        protected virtual void OnEnable()
        {
            m_DataPersister = (IDataPersister) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DataPersisterGUI(m_DataPersister);
        }

        public static void DataPersisterGUI(IDataPersister dataPersister)
        {
            var dataSettings = dataPersister.GetDataSettings();

            var persistenceType =
                (DataSettings.PersistenceType) EditorGUILayout.EnumPopup("Persistence Type",
                    dataSettings.persistenceType);
            var dataTag = EditorGUILayout.TextField("Data Tag", dataSettings.dataTag);

            dataPersister.SetDataSettings(dataTag, persistenceType);
        }
    }
}