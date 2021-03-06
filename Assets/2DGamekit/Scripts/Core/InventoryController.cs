﻿#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Gamekit2D
{
    public class InventoryController : MonoBehaviour, IDataPersister
    {
        private readonly HashSet<string> m_InventoryItems = new HashSet<string>();
        public DataSettings dataSettings;

        public InventoryEvent[] inventoryEvents;

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
            return new Data<HashSet<string>>(m_InventoryItems);
        }

        public void LoadData(Data data)
        {
            var inventoryData = (Data<HashSet<string>>) data;

            foreach (var i in inventoryData.value)
                AddItem(i);

            if (OnInventoryLoaded != null)
                OnInventoryLoaded();
        }

        public event Action OnInventoryLoaded;


        //Debug function useful in editor during play mode to print in console all objects in that InventoyController
        [ContextMenu("Dump")]
        private void Dump()
        {
            foreach (var item in m_InventoryItems) Debug.Log(item);
        }

        private void OnEnable()
        {
            PersistentDataManager.RegisterPersister(this);
        }

        private void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        public void AddItem(string key)
        {
            if (!m_InventoryItems.Contains(key))
            {
                //Debug.Log("AddItem - key : " + key);

                m_InventoryItems.Add(key);
                var ev = GetInventoryEvent(key);
                if (ev != null) ev.OnAdd.Invoke();
            }

            /*else
            {
                Debug.Log("Already have key : " + key);
            }*/
        }

        public void RemoveItem(string key)
        {
            if (m_InventoryItems.Contains(key))
            {
                /*Debug.Log("RemoveItem - key : " + key);*/

                var ev = GetInventoryEvent(key);
                if (ev != null) ev.OnRemove.Invoke();
                m_InventoryItems.Remove(key);
            }

            /*else
            {
                Debug.Log("Does not have key : " + key);
            }*/
        }

        public bool HasItem(string key)
        {
            return m_InventoryItems.Contains(key);
        }

        public void Clear()
        {
            m_InventoryItems.Clear();
        }

        private InventoryEvent GetInventoryEvent(string key)
        {
            foreach (var iv in inventoryEvents)
                if (iv.key == key)
                    return iv;
            return null;
        }

        [Serializable]
        public class InventoryEvent
        {
            public string key;
            public UnityEvent OnAdd, OnRemove;
        }

        [Serializable]
        public class InventoryChecker
        {
            public string[] inventoryItems;
            public UnityEvent OnHasItem, OnDoesNotHaveItem;

            public bool CheckInventory(InventoryController inventory)
            {
                if (inventory != null)
                {
                    for (var i = 0; i < inventoryItems.Length; i++)
                        if (!inventory.HasItem(inventoryItems[i]))
                        {
                            OnDoesNotHaveItem.Invoke();
                            return false;
                        }

                    OnHasItem.Invoke();
                    return true;
                }

                return false;
            }
        }
    }
}