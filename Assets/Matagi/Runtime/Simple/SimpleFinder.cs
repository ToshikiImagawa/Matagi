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
        /// It searches and retrieves the component from the descendants of the base component based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="com">The base component for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
        /// <returns>
        ///   <para>A Component of the matching type, otherwise null if no Component is found.</para>
        /// </returns>
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
        /// It searches and retrieves the component from the descendants of the base gameObject based on the path.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="obj">The base gameObject for the search.</param>
        /// <param name="path">The name or path of the GameObject to be searched.</param>
        /// <param name="loaded">The delegate that receives the component to be searched.</param>
        /// <param name="includeInactive">Whether to include inactive child GameObjects in the search.</param>
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
                IComponentCache localComponentCache = obj.GetComponent<SimpleLocalComponentCache>() ??
                                                      obj.GetComponentInParent<SimpleLocalComponentCache>()
                                                      ?? obj.Root().AddComponent<SimpleLocalComponentCache>();

                component = localComponentCache?.GetComponent<T>(obj, path, includeInactive);
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
        /// <returns>
        ///   <para>A Component of the matching type, otherwise null if no Component is found.</para>
        /// </returns>
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

            IComponentCache localComponentCache = obj.GetComponent<SimpleLocalComponentCache>() ??
                                                  obj.GetComponentInParent<SimpleLocalComponentCache>()
                                                  ?? obj.Root().AddComponent<SimpleLocalComponentCache>();

            return localComponentCache?.GetComponent<T>(obj, path, includeInactive);
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
    }
}