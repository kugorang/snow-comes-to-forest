﻿#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#endregion

namespace Gamekit2D
{
    /// <summary>
    ///     This class is used as a cache of components on the same gameobjects as colliders.
    ///     The intention is that when a collider is found - by raycast or otherwise - the components being sought by those
    ///     raycasts can be referenced without the need for GetComponent calls.
    /// </summary>
    public class PhysicsHelper : MonoBehaviour
    {
        private static PhysicsHelper s_Instance;

        private readonly Dictionary<Collider2D, AudioSurface> m_AudioSurfaceCache =
            new Dictionary<Collider2D, AudioSurface>();

        private readonly Dictionary<Collider2D, MovingPlatform> m_MovingPlatformCache =
            new Dictionary<Collider2D, MovingPlatform>();

        private readonly Dictionary<Collider2D, PlatformEffector2D> m_PlatformEffectorCache =
            new Dictionary<Collider2D, PlatformEffector2D>();

        private readonly Dictionary<Collider2D, Tilemap> m_TilemapCache = new Dictionary<Collider2D, Tilemap>();

        private static PhysicsHelper Instance
        {
            get
            {
                if (s_Instance != null)
                    return s_Instance;

                s_Instance = FindObjectOfType<PhysicsHelper>();

                if (s_Instance != null)
                    return s_Instance;

                Create();

                return s_Instance;
            }
            set { s_Instance = value; }
        }

        private static void Create()
        {
            var physicsHelperGameObject = new GameObject("PhysicsHelper");
            s_Instance = physicsHelperGameObject.AddComponent<PhysicsHelper>();
        }

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            PopulateColliderDictionary(m_MovingPlatformCache);
            PopulateColliderDictionary(m_PlatformEffectorCache);
            PopulateColliderDictionary(m_TilemapCache);
            PopulateColliderDictionary(m_AudioSurfaceCache);
        }

        protected void PopulateColliderDictionary<TComponent>(Dictionary<Collider2D, TComponent> dict)
            where TComponent : Component
        {
            var components = FindObjectsOfType<TComponent>();

            for (var i = 0; i < components.Length; i++)
            {
                var componentColliders = components[i].GetComponents<Collider2D>();

                for (var j = 0; j < componentColliders.Length; j++) dict.Add(componentColliders[j], components[i]);
            }
        }

        public static bool ColliderHasMovingPlatform(Collider2D collider)
        {
            return Instance.m_MovingPlatformCache.ContainsKey(collider);
        }

        public static bool ColliderHasPlatformEffector(Collider2D collider)
        {
            return Instance.m_PlatformEffectorCache.ContainsKey(collider);
        }

        public static bool ColliderHasTilemap(Collider2D collider)
        {
            return Instance.m_TilemapCache.ContainsKey(collider);
        }

        public static bool ColliderHasAudioSurface(Collider2D collider)
        {
            return Instance.m_AudioSurfaceCache.ContainsKey(collider);
        }

        public static bool TryGetMovingPlatform(Collider2D collider, out MovingPlatform movingPlatform)
        {
            return Instance.m_MovingPlatformCache.TryGetValue(collider, out movingPlatform);
        }

        public static bool TryGetPlatformEffector(Collider2D collider, out PlatformEffector2D platformEffector)
        {
            return Instance.m_PlatformEffectorCache.TryGetValue(collider, out platformEffector);
        }

        public static bool TryGetTilemap(Collider2D collider, out Tilemap tilemap)
        {
            return Instance.m_TilemapCache.TryGetValue(collider, out tilemap);
        }

        public static bool TryGetAudioSurface(Collider2D collider, out AudioSurface audioSurface)
        {
            return Instance.m_AudioSurfaceCache.TryGetValue(collider, out audioSurface);
        }

        public static TileBase FindTileForOverride(Collider2D collider, Vector2 position, Vector2 direction)
        {
            Tilemap tilemap;
            if (TryGetTilemap(collider, out tilemap))
                return tilemap.GetTile(tilemap.WorldToCell(position + direction * 0.4f));

            AudioSurface audioSurface;
            if (TryGetAudioSurface(collider, out audioSurface)) return audioSurface.tile;

            return null;
        }
    }
}