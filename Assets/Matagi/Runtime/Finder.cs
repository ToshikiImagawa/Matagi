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
            CacheType cacheType = CacheType.Local
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
            CacheType cacheType = CacheType.Local
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
            CacheType cacheType = CacheType.Local
        ) where T : Component
        {
            T component;
            if (cacheType == CacheType.Static || !Application.isPlaying)
            {
                lock (CacheDictLock)
                {
                    component = FinderUtil.GetComponent<T>(obj, path, includeInactive, CacheDict);
                }
            }
            else
            {
                var localComponentCache = cacheType == CacheType.Local
                    ? obj.GetComponent<LocalComponentCache>() ??
                      obj.GetComponentInParent<LocalComponentCache>() ??
                      obj.Root().AddComponent<LocalComponentCache>()
                    : obj.scene.GetRootGameObjects()
                        .Select(root => root.GetComponent<LocalComponentCache>())
                        .FirstOrDefault(cache => cache != null) ?? CreateLocalComponentCache(obj);

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
            CacheType cacheType = CacheType.Local
        ) where T : Component
        {
            if (obj == null) return null;
            if (cacheType != CacheType.Static && Application.isPlaying)
            {
                var rootScene = obj.scene;

                if (!rootScene.IsValid() || !rootScene.isLoaded)
                {
                    return FinderUtil.GetComponent<T>(
                        obj,
                        path,
                        includeInactive,
                        new Dictionary<string, Component>()
                    );
                }

                var localComponentCache = cacheType == CacheType.Local
                    ? obj.GetComponent<LocalComponentCache>() ??
                      obj.GetComponentInParent<LocalComponentCache>() ??
                      obj.Root().AddComponent<LocalComponentCache>()
                    : obj.scene.GetRootGameObjects()
                        .Select(root => root.GetComponent<LocalComponentCache>())
                        .FirstOrDefault(cache => cache != null) ?? CreateLocalComponentCache(obj);

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

        public enum CacheType
        {
            /// <summary>
            /// 検索対象のRootのGameObjectにキャッシュします
            /// </summary>
            Local,

            /// <summary>
            /// シーンで１つのGameObjectにキャッシュします
            /// </summary>
            Scene,

            /// <summary>
            /// Staticにキャッシュします
            /// </summary>
            Static
        }

        private static LocalComponentCache CreateLocalComponentCache(GameObject obj)
        {
            var localComponentCache = new GameObject("[LocalComponentCache]").AddComponent<LocalComponentCache>();
            SceneManager.MoveGameObjectToScene(localComponentCache.gameObject, obj.scene);
            return localComponentCache;
        }
    }
}