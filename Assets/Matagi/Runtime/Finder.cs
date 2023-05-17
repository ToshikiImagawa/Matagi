// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 現在のGameObjectの子からNAMEというGOを探し、
        /// そのGOの持っているT型のコンポーネントを取り出す
        /// 
        /// キャッシュが効く。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="com"></param>
        /// <param name="remoteGameObjectName"></param>
        /// <param name="loaded"></param>
        /// <param name="includeInactive"></param>
        /// <param name="staticCache"></param>
        public static void FindComponent<T>(
            this Component com,
            string remoteGameObjectName,
            Action<T> loaded,
            bool includeInactive = false,
            bool staticCache = false
        ) where T : Component
        {
            FindComponent(
                com.gameObject,
                remoteGameObjectName,
                loaded,
                includeInactive,
                staticCache
            );
        }

        /// <summary>
        /// 現在のGameObjectの子からNAMEというGOを探し、
        /// そのGOの持っているT型のコンポーネントを取り出す
        /// 
        /// キャッシュが効く。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="com"></param>
        /// <param name="remoteGameObjectName"></param>
        /// <param name="includeInactive"></param>
        /// <param name="staticCache"></param>
        /// <returns></returns>
        public static T FindComponent<T>(
            this Component com,
            string remoteGameObjectName = null,
            bool includeInactive = false,
            bool staticCache = false
        ) where T : Component
        {
            if (com == null) return null;
            var obj = com.gameObject;
            return FindComponent<T>(
                obj,
                remoteGameObjectName,
                includeInactive,
                staticCache
            );
        }

        /// <summary>
        /// 現在のGameObjectの子からNAMEというGOを探し、
        /// そのGOの持っているT型のコンポーネントを取り出す
        ///
        /// キャッシュが効く。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="remoteGameObjectName"></param>
        /// <param name="loaded"></param>
        /// <param name="includeInactive"></param>
        /// <param name="staticCache"></param>
        public static void FindComponent<T>(
            this GameObject obj,
            string remoteGameObjectName,
            Action<T> loaded,
            bool includeInactive = false,
            bool staticCache = false
        ) where T : Component
        {
            T component;
            if (staticCache || !Application.isPlaying)
            {
                lock (CacheDictLock)
                {
                    component = FinderUtil.GetComponent<T>(obj, remoteGameObjectName, includeInactive, CacheDict);
                }
            }
            else
            {
                LocalComponentCache localComponentCache = null;
                foreach (var root in obj.scene.GetRootGameObjects())
                {
                    localComponentCache = root.GetComponent<LocalComponentCache>();
                    if (localComponentCache != null) break;
                }

                if (localComponentCache == null)
                {
                    localComponentCache = new GameObject("[LocalComponentCache]").AddComponent<LocalComponentCache>();
                    SceneManager.MoveGameObjectToScene(localComponentCache.gameObject, obj.scene);
                }

                component = localComponentCache.GetComponent<T>(obj, remoteGameObjectName, includeInactive);
            }

            if (component != null)
            {
                loaded?.Invoke(component);
                return;
            }

            // not found.
            Debug.LogError($"{obj.name} failed to found component:{typeof(T)} from gameObject:{remoteGameObjectName}");
        }

        /// <summary>
        /// 現在のGameObjectの子からNAMEというGOを探し、
        /// そのGOの持っているT型のコンポーネントを取り出す
        ///
        /// キャッシュが効く。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="remoteGameObjectName"></param>
        /// <param name="includeInactive"></param>
        /// <param name="staticCache"></param>
        /// <returns></returns>
        public static T FindComponent<T>(
            this GameObject obj,
            string remoteGameObjectName = null,
            bool includeInactive = false,
            bool staticCache = false
        ) where T : Component
        {
            if (obj == null) return null;
            if (!staticCache && Application.isPlaying)
            {
                LocalComponentCache localComponentCache = null;
                var rootScene = obj.scene;

                if (!rootScene.IsValid() || !rootScene.isLoaded)
                    return FinderUtil.GetComponent<T>(obj, remoteGameObjectName, includeInactive,
                        new Dictionary<string, Component>());
                foreach (var root in rootScene.GetRootGameObjects())
                {
                    localComponentCache = root.GetComponent<LocalComponentCache>();
                    if (localComponentCache != null) break;
                }

                if (localComponentCache != null)
                    return localComponentCache.GetComponent<T>(obj, remoteGameObjectName, includeInactive);
                localComponentCache = new GameObject("[LocalComponentCache]").AddComponent<LocalComponentCache>();
                SceneManager.MoveGameObjectToScene(localComponentCache.gameObject, obj.scene);

                return localComponentCache.GetComponent<T>(obj, remoteGameObjectName, includeInactive);
            }

            lock (CacheDictLock)
            {
                return FinderUtil.GetComponent<T>(obj, remoteGameObjectName, includeInactive, CacheDict);
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
    }
}