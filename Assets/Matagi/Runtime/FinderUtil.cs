// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Matagi
{
    internal static class FinderUtil
    {
        internal static TComponent GetComponent<TComponent>(
            GameObject findRoot,
            string path,
            bool includeInactive,
            IDictionary<string, Component> cacheDict
        ) where TComponent : Component
        {
            if (string.IsNullOrEmpty(path))
            {
                path = findRoot.name;
            }

            var key = GetKey<TComponent>(findRoot, path);
            if (cacheDict.ContainsKey(key))
            {
                var component = cacheDict[key] as TComponent;
                if (component != null) return component;
                cacheDict.Remove(key);
            }

            if (findRoot.name == path)
            {
                var component = findRoot.GetComponent<TComponent>();
                if (component != null)
                {
                    cacheDict[GetKey<TComponent>(findRoot, path)] = component;

                    return component;
                }
            }

            var components = findRoot.GetComponentsInChildren<TComponent>(includeInactive);
            var searchKeys = path.Split('/');

            TComponent hitComponent = null;
            foreach (var component in components)
            {
                var name = component.gameObject.name;
                var parent = component.transform.parent;
                var hit = true;
                for (var i = searchKeys.Length - 1; i >= 0; i--)
                {
                    var searchKey = searchKeys[i];
                    if (name != searchKey)
                    {
                        hit = false;
                        break;
                    }

                    if (parent != null)
                    {
                        name = parent.name;
                        parent = parent.parent;
                    }
                    else
                    {
                        name = string.Empty;
                    }
                }

                if (!hit) continue;
                hitComponent = component;
                break;
            }

            if (hitComponent == null) return null;
            cacheDict[GetKey<TComponent>(findRoot, path)] = hitComponent;
            return hitComponent;
        }

        private static string GetKey<TComponent>(Object findRoot, string path) where TComponent : Component
        {
            return $"{findRoot.GetInstanceID()}_{path}_{typeof(TComponent)}";
        }
    }
}