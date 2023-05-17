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
        public static void FindComponent<T>(
            this Component com,
            string remoteGameObjectName,
            Action<T> loaded,
            bool includeInactive = false
        ) where T : Component
        {
            FindComponent(
                com.gameObject,
                remoteGameObjectName,
                loaded,
                includeInactive
            );
        }

        public static T FindComponent<T>(
            this Component com,
            string remoteGameObjectName = null,
            bool includeInactive = false
        ) where T : Component
        {
            if (com == null) return null;
            var obj = com.gameObject;
            return FindComponent<T>(
                obj,
                remoteGameObjectName,
                includeInactive
            );
        }

        public static void FindComponent<T>(
            this GameObject obj,
            string remoteGameObjectName,
            Action<T> loaded,
            bool includeInactive = false
        ) where T : Component
        {
            T component;
            if (!Application.isPlaying)
            {
                component = FinderUtil.GetComponent<T>(
                    obj,
                    remoteGameObjectName,
                    includeInactive,
                    new Dictionary<string, Component>()
                );
            }
            else
            {
                SimpleLocalComponentCache localComponentCache = null;
                foreach (var root in obj.scene.GetRootGameObjects())
                {
                    localComponentCache = root.GetComponent<SimpleLocalComponentCache>();
                    if (localComponentCache != null) break;
                }

                if (localComponentCache == null)
                {
                    throw new InvalidOperationException(
                        $"{nameof(SimpleLocalComponentCache)} not found. Please place a game object with {nameof(SimpleLocalComponentCache)} attached to the root of the scene."
                    );
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

        public static T FindComponent<T>(
            this GameObject obj,
            string remoteGameObjectName = null,
            bool includeInactive = false
        ) where T : Component
        {
            if (obj == null) return null;
            if (Application.isPlaying)
            {
                SimpleLocalComponentCache localComponentCache = null;
                var rootScene = obj.scene;

                if (!rootScene.IsValid() || !rootScene.isLoaded)
                    return FinderUtil.GetComponent<T>(
                        obj,
                        remoteGameObjectName,
                        includeInactive,
                        new Dictionary<string, Component>()
                    );
                foreach (var root in rootScene.GetRootGameObjects())
                {
                    localComponentCache = root.GetComponent<SimpleLocalComponentCache>();
                    if (localComponentCache != null) break;
                }

                if (localComponentCache == null)
                {
                    throw new InvalidOperationException(
                        $"{nameof(SimpleLocalComponentCache)} not found. Please place a game object with {nameof(SimpleLocalComponentCache)} attached to the root of the scene."
                    );
                }

                return localComponentCache.GetComponent<T>(obj, remoteGameObjectName, includeInactive);
            }

            return FinderUtil.GetComponent<T>(
                obj,
                remoteGameObjectName,
                includeInactive,
                new Dictionary<string, Component>()
            );
        }
    }
}