// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using Matagi.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Matagi.Simple
{
    public static class SimpleFinder
    {
        /// <summary>
        /// 現在のGameObjectの子孫からPATHというGameObjectを探し、
        /// そのGameObjectの持っているT型のコンポーネントを取得します
        /// </summary>
        /// <param name="com"></param>
        /// <param name="path"></param>
        /// <param name="loaded"></param>
        /// <param name="includeInactive"></param>
        /// <typeparam name="T"></typeparam>
        public static void FindComponent<T>(
            this Component com,
            string path,
            Action<T> loaded,
            bool includeInactive = false
        ) where T : Component
        {
            FindComponent(
                com.gameObject,
                path,
                loaded,
                includeInactive
            );
        }

        /// <summary>
        /// 現在のGameObjectの子孫からPATHというGameObjectを探し、
        /// そのGameObjectの持っているT型のコンポーネントを取得します
        /// </summary>
        /// <param name="com"></param>
        /// <param name="path"></param>
        /// <param name="includeInactive"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindComponent<T>(
            this Component com,
            string path = null,
            bool includeInactive = false
        ) where T : Component
        {
            if (com == null) return null;
            var obj = com.gameObject;
            return FindComponent<T>(
                obj,
                path,
                includeInactive
            );
        }

        /// <summary>
        /// 現在のGameObjectの子孫からPATHというGameObjectを探し、
        /// そのGameObjectの持っているT型のコンポーネントを取得します
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <param name="loaded"></param>
        /// <param name="includeInactive"></param>
        /// <typeparam name="T"></typeparam>
        public static void FindComponent<T>(
            this GameObject obj,
            string path,
            Action<T> loaded,
            bool includeInactive = false
        ) where T : Component
        {
            T component;
            if (!Application.isPlaying)
            {
                component = FinderUtil.GetComponent<T>(
                    obj,
                    path,
                    includeInactive,
                    new Dictionary<string, Component>()
                );
            }
            else
            {
                var localComponentCache = obj.GetComponent<SimpleLocalComponentCache>() ??
                                          obj.GetComponentInParent<SimpleLocalComponentCache>()
                                          ?? obj.Root().AddComponent<SimpleLocalComponentCache>();

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
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <param name="includeInactive"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindComponent<T>(
            this GameObject obj,
            string path = null,
            bool includeInactive = false
        ) where T : Component
        {
            if (obj == null) return null;
            if (!Application.isPlaying)
            {
                return FinderUtil.GetComponent<T>(
                    obj,
                    path,
                    includeInactive,
                    new Dictionary<string, Component>()
                );
            }

            var localComponentCache = obj.GetComponent<SimpleLocalComponentCache>() ??
                                      obj.GetComponentInParent<SimpleLocalComponentCache>()
                                      ?? obj.Root().AddComponent<SimpleLocalComponentCache>();

            return localComponentCache.GetComponent<T>(obj, path, includeInactive);
        }
    }
}