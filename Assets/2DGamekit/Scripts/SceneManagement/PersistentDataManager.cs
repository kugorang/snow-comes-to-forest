﻿#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class PersistentDataManager : MonoBehaviour
    {
        protected static PersistentDataManager instance;
        protected static bool quitting;

        protected HashSet<IDataPersister> m_DataPersisters = new HashSet<IDataPersister>();
        protected Dictionary<string, Data> m_Store = new Dictionary<string, Data>();

        public static PersistentDataManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = FindObjectOfType<PersistentDataManager>();
                if (instance != null)
                    return instance;

                Create();
                return instance;
            }
        }

        public static PersistentDataManager Create()
        {
            var dataManagerGameObject = new GameObject("PersistentDataManager");
            DontDestroyOnLoad(dataManagerGameObject);
            instance = dataManagerGameObject.AddComponent<PersistentDataManager>();
            return instance;
        }

        private event Action schedule;

        private void Update()
        {
            if (schedule != null)
            {
                schedule();
                schedule = null;
            }
        }

        private void Awake()
        {
            if (Instance != this)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (instance == this)
                quitting = true;
        }

        public static void RegisterPersister(IDataPersister persister)
        {
            var ds = persister.GetDataSettings();
            if (!string.IsNullOrEmpty(ds.dataTag)) Instance.Register(persister);
        }

        public static void UnregisterPersister(IDataPersister persister)
        {
            if (!quitting) Instance.Unregister(persister);
        }

        public static void SaveAllData()
        {
            Instance.SaveAllDataInternal();
        }

        public static void LoadAllData()
        {
            Instance.LoadAllDataInternal();
        }

        public static void ClearPersisters()
        {
            Instance.m_DataPersisters.Clear();
        }

        public static void SetDirty(IDataPersister dp)
        {
            Instance.Save(dp);
        }

        protected void SaveAllDataInternal()
        {
            foreach (var dp in m_DataPersisters) Save(dp);
        }

        protected void Register(IDataPersister persister)
        {
            schedule += () => { m_DataPersisters.Add(persister); };
        }

        protected void Unregister(IDataPersister persister)
        {
            schedule += () => m_DataPersisters.Remove(persister);
        }

        protected void Save(IDataPersister dp)
        {
            var dataSettings = dp.GetDataSettings();
            if (dataSettings.persistenceType == DataSettings.PersistenceType.ReadOnly ||
                dataSettings.persistenceType == DataSettings.PersistenceType.DoNotPersist)
                return;
            if (!string.IsNullOrEmpty(dataSettings.dataTag)) m_Store[dataSettings.dataTag] = dp.SaveData();
        }

        protected void LoadAllDataInternal()
        {
            schedule += () =>
            {
                foreach (var dp in m_DataPersisters)
                {
                    var dataSettings = dp.GetDataSettings();
                    if (dataSettings.persistenceType == DataSettings.PersistenceType.WriteOnly ||
                        dataSettings.persistenceType == DataSettings.PersistenceType.DoNotPersist)
                        continue;
                    if (!string.IsNullOrEmpty(dataSettings.dataTag))
                        if (m_Store.ContainsKey(dataSettings.dataTag))
                            dp.LoadData(m_Store[dataSettings.dataTag]);
                }
            };
        }
    }
}