// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Matagi.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Matagi
{
    public static class Finder
    {
        private static readonly Dictionary<string, Component> CacheDict = new();
        private static readonly Dictionary<int, SceneComponentCache> SceneComponentCacheDict = new();
        private static readonly Dictionary<int, LocalComponentCache> LocalComponentCacheDict = new();
        private static readonly Dictionary<int, int> RootGameObjectLocalComponentCacheMap = new();
        private static readonly object CacheDictLock = new();

        /// <summary>
        /// Default cache type
        /// </summary>
        public static CacheType DefaultCacheType = CacheType.Scene;

        /// <summary>
        /// It searches and retrieves the component from the descendants of the base component based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="com">The base component for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="loaded">The delegate that receives the component to be searched.</param>
        /// <param name="cache">The cache dictionary.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        public static void FindComponent<T>(
            this Component com,
            string path,
            Action<T> loaded,
            Dictionary<string, Component> cache,
            bool includeInactive = false
        ) where T : Component
        {
            FindComponent(
                com.gameObject,
                path,
                loaded,
                cache,
                includeInactive
            );
        }

        /// <summary>
        /// It searches and retrieves the component from the descendants of the base gameObject based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="com">The base component for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="cache">The cache dictionary.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <returns>
        ///   <para>A Component of the matching type, otherwise null if no Component is found.</para>
        /// </returns>
        public static T FindComponent<T>(
            this Component com,
            Dictionary<string, Component> cache,
            string path = null,
            bool includeInactive = false
        ) where T : Component
        {
            if (com == null) return null;
            var obj = com.gameObject;
            return FindComponent<T>(
                obj,
                cache,
                path,
                includeInactive
            );
        }

        /// <summary>
        /// It searches and retrieves the component from the descendants of the base component based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="com">The base component for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="loaded">The delegate that receives the component to be searched.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <param name="cacheType">cache type.</param>
        public static void FindComponent<T>(
            this Component com,
            string path,
            Action<T> loaded,
            bool includeInactive = false,
            CacheType? cacheType = null
        ) where T : Component
        {
            FindComponent(
                com.gameObject,
                path,
                loaded,
                includeInactive,
                cacheType
            );
        }

        /// <summary>
        /// It searches and retrieves the component from the descendants of the base component based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="com">The base component for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <param name="cacheType">cache type.</param>
        /// <returns>
        ///   <para>A Component of the matching type, otherwise null if no Component is found.</para>
        /// </returns>
        public static T FindComponent<T>(
            this Component com,
            string path = null,
            bool includeInactive = false,
            CacheType? cacheType = null
        ) where T : Component
        {
            if (com == null) return null;
            var obj = com.gameObject;
            return FindComponent<T>(
                obj,
                path,
                includeInactive,
                cacheType
            );
        }

        /// <summary>
        /// It searches and retrieves the component from the descendants of the base gameObject based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="obj">The base gameObject for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="loaded">The delegate that receives the component to be searched.</param>
        /// <param name="cache">The cache dictionary.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        public static void FindComponent<T>(
            this GameObject obj,
            string path,
            Action<T> loaded,
            Dictionary<string, Component> cache,
            bool includeInactive = false
        ) where T : Component
        {
            if (obj == null) return;
            if (cache == null) return;
            var component = FinderUtil.GetComponent<T>(obj, path, includeInactive, cache);
            if (component != null)
            {
                loaded?.Invoke(component);
                return;
            }

            // not found.
            Debug.LogError($"{obj.name} failed to found component:{typeof(T)} from gameObject:{path}");
        }

        /// <summary>
        /// It searches and retrieves the component from the descendants of the base gameObject based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="obj">The base gameObject for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="cache">The cache dictionary.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <returns>
        ///   <para>A Component of the matching type, otherwise null if no Component is found.</para>
        /// </returns>
        public static T FindComponent<T>(
            this GameObject obj,
            Dictionary<string, Component> cache,
            string path = null,
            bool includeInactive = false
        ) where T : Component
        {
            if (obj == null || cache == null) return null;
            return FinderUtil.GetComponent<T>(obj, path, includeInactive, cache);
        }

        /// <summary>
        /// It searches and retrieves the component from the descendants of the base gameObject based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="obj">The base gameObject for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="loaded">The delegate that receives the component to be searched.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <param name="cacheType">cache type.</param>
        public static void FindComponent<T>(
            this GameObject obj,
            string path,
            Action<T> loaded,
            bool includeInactive = false,
            CacheType? cacheType = null
        ) where T : Component
        {
            if (obj == null) return;
            var nonnullCacheType = cacheType ?? DefaultCacheType;
            T component;
            if (nonnullCacheType == CacheType.Static || !Application.isPlaying)
            {
                lock (CacheDictLock)
                {
                    component = FinderUtil.GetComponent<T>(obj, path, includeInactive, CacheDict);
                }
            }
            else
            {
                IComponentCache componentCache = nonnullCacheType == CacheType.Local
                    ? GetOrCreateLocalComponentCache(obj)
                    : GetOrCreateSceneComponentCache(obj);

                component = componentCache?.GetComponent<T>(obj, path, includeInactive);
            }

            if (component != null)
            {
                loaded?.Invoke(component);
                return;
            }

            // not found.
            Debug.LogError($"{obj.name} failed to found component:{typeof(T)} from gameObject:{path}");
        }

        /// <summary>
        /// It searches and retrieves the component from the descendants of the base gameObject based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="obj">The base gameObject for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <param name="cacheType">cache type.</param>
        /// <returns>
        ///   <para>A Component of the matching type, otherwise null if no Component is found.</para>
        /// </returns>
        public static T FindComponent<T>(
            this GameObject obj,
            string path = null,
            bool includeInactive = false,
            CacheType? cacheType = null
        ) where T : Component
        {
            if (obj == null) return null;
            var nonnullCacheType = cacheType ?? DefaultCacheType;
            if (nonnullCacheType != CacheType.Static && Application.isPlaying)
            {
                IComponentCache componentCache = nonnullCacheType == CacheType.Local
                    ? GetOrCreateLocalComponentCache(obj)
                    : GetOrCreateSceneComponentCache(obj);

                return componentCache?.GetComponent<T>(obj, path, includeInactive);
            }

            lock (CacheDictLock)
            {
                return FinderUtil.GetComponent<T>(obj, path, includeInactive, CacheDict);
            }
        }

        /// <summary>
        /// Clear all caches.
        /// </summary>
        public static void ClearCache()
        {
            lock (CacheDictLock)
            {
                CacheDict.Clear();
            }
        }

        /// <summary>
        /// Clear the cache based on the InstanceId of the base object.
        /// </summary>
        /// <param name="instanceId"></param>
        public static void ClearCacheFromParentInstanceId(string instanceId)
        {
            lock (CacheDictLock)
            {
                var fitKeys = CacheDict.Keys.Where(_ => _.StartsWith(instanceId + "_")).ToArray();
                foreach (var fitKey in fitKeys)
                {
                    CacheDict.Remove(fitKey);
                }
            }
        }

        /// <summary>
        /// Clear the cache from the base object.
        /// </summary>
        /// <param name="obj"></param>
        public static void ClearCacheFromParentObject(Object obj)
        {
            ClearCacheFromParentInstanceId(obj.GetInstanceID().ToString());
        }

        public static void ClearComponentCacheMap()
        {
            LocalComponentCacheDict.Clear();
            SceneComponentCacheDict.Clear();
            RootGameObjectLocalComponentCacheMap.Clear();
        }

        internal static void RemoveLocalComponentCache(int instanceId)
        {
            LocalComponentCacheDict.Remove(instanceId);
            var removeList = RootGameObjectLocalComponentCacheMap
                .Where(keyValue => keyValue.Value == instanceId)
                .Select(keyValue => keyValue.Key)
                .ToArray();
            foreach (var id in removeList)
            {
                RootGameObjectLocalComponentCacheMap.Remove(id);
            }
        }

        internal static void RemoveSceneComponentCache(int sceneHandle)
        {
            SceneComponentCacheDict.Remove(sceneHandle);
        }

        private static SceneComponentCache CreateSceneComponentCache(GameObject obj)
        {
            var localComponentCache = new GameObject("[SceneComponentCache]").AddComponent<SceneComponentCache>();
            if (localComponentCache.gameObject.scene != obj.scene)
            {
                SceneManager.MoveGameObjectToScene(localComponentCache.gameObject, obj.scene);
            }

            return localComponentCache;
        }

        private static LocalComponentCache GetOrCreateLocalComponentCache(GameObject obj)
        {
            var id = obj.GetInstanceID();
            if (RootGameObjectLocalComponentCacheMap.TryGetValue(id, out var key))
            {
                return !LocalComponentCacheDict.TryGetValue(key, out var localComponentCache)
                    ? LocalComponentCacheDict[key] = GetLocalComponentCache()
                    : localComponentCache;
            }
            else
            {
                var localComponentCache = GetLocalComponentCache();
                var componentCacheId = localComponentCache.gameObject.GetInstanceID();
                LocalComponentCacheDict[componentCacheId] = localComponentCache;
                RootGameObjectLocalComponentCacheMap[id] = componentCacheId;
                return localComponentCache;
            }

            LocalComponentCache GetLocalComponentCache()
            {
                return obj.GetComponent<LocalComponentCache>() ??
                       obj.GetComponentInParent<LocalComponentCache>() ??
                       obj.Root().AddComponent<LocalComponentCache>();
            }
        }

        private static SceneComponentCache GetOrCreateSceneComponentCache(GameObject obj)
        {
            var key = obj.scene.handle;
            return !SceneComponentCacheDict.TryGetValue(key, out var sceneComponentCache)
                ? SceneComponentCacheDict[key] = obj.scene.GetRootGameObjects()
                    .Select(root => root.GetComponent<SceneComponentCache>())
                    .FirstOrDefault(cache => cache != null) ?? CreateSceneComponentCache(obj)
                : sceneComponentCache;
        }
    }
}