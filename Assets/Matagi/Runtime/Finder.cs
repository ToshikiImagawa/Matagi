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
        private static readonly object CacheDictLock = new();

        /// <summary>
        /// デフォルトのCacheType
        /// </summary>
        public static CacheType DefaultCacheType = CacheType.Local;

        /// <summary>
        /// 現在のGameObjectの子孫からPATHというGameObjectを探し、
        /// そのGameObjectの持っているT型のコンポーネントを取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="com"></param>
        /// <param name="path"></param>
        /// <param name="loaded"></param>
        /// <param name="includeInactive"></param>
        /// <param name="cacheType"></param>
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
        /// 現在のGameObjectの子孫からPATHというGameObjectを探し、
        /// そのGameObjectの持っているT型のコンポーネントを取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="com"></param>
        /// <param name="path"></param>
        /// <param name="includeInactive"></param>
        /// <param name="cacheType"></param>
        /// <returns></returns>
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
        /// 現在のGameObjectの子孫からPATHというGameObjectを探し、
        /// そのGameObjectの持っているT型のコンポーネントを取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <param name="loaded"></param>
        /// <param name="includeInactive"></param>
        /// <param name="cacheType"></param>
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
                IComponentCache localComponentCache = nonnullCacheType == CacheType.Local
                    ? obj.GetComponent<LocalComponentCache>() ??
                      obj.GetComponentInParent<LocalComponentCache>() ??
                      obj.Root().AddComponent<LocalComponentCache>()
                    : obj.scene.GetRootGameObjects()
                        .Select(root => root.GetComponent<SceneComponentCache>())
                        .FirstOrDefault(cache => cache != null) ?? CreateSceneComponentCache(obj);

                component = localComponentCache.GetComponent<T>(obj, path, includeInactive);
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
        /// 現在のGameObjectの子孫からPATHというGameObjectを探し、
        /// そのGameObjectの持っているT型のコンポーネントを取得します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <param name="includeInactive"></param>
        /// <param name="cacheType"></param>
        /// <returns></returns>
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
                IComponentCache localComponentCache = nonnullCacheType == CacheType.Local
                    ? obj.GetComponent<LocalComponentCache>() ??
                      obj.GetComponentInParent<LocalComponentCache>() ??
                      obj.Root().AddComponent<LocalComponentCache>()
                    : obj.scene.GetRootGameObjects()
                        .Select(root => root.GetComponent<SceneComponentCache>())
                        .FirstOrDefault(cache => cache != null) ?? CreateSceneComponentCache(obj);

                return localComponentCache.GetComponent<T>(obj, path, includeInactive);
            }

            lock (CacheDictLock)
            {
                return FinderUtil.GetComponent<T>(obj, path, includeInactive, CacheDict);
            }
        }

        /// <summary>
        /// 全キャッシュクリア
        /// </summary>
        public static void CacheClear()
        {
            lock (CacheDictLock)
            {
                CacheDict.Clear();
            }
        }

        /// <summary>
        /// 検索もとGameObjectのInstanceIdよりキャッシュクリア
        /// </summary>
        /// <param name="instanceId"></param>
        public static void CacheClearFromParentInstanceId(string instanceId)
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
        /// 検索もとGameObjectよりキャッシュクリア
        /// </summary>
        /// <param name="obj"></param>
        public static void CacheClearFromParentObject(Object obj)
        {
            CacheClearFromParentInstanceId(obj.GetInstanceID().ToString());
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
    }
}