// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Matagi.Core;
using UnityEngine;

namespace Matagi
{
    [DisallowMultipleComponent]
    public class LocalComponentCache : MonoBehaviour, IComponentCache
    {
#if UNITY_EDITOR
        [HideInInspector] public bool visualize;
        public Color defaultColor = Color.yellow;
        public Color selectedColor = Color.red;
        private bool _updateCacheDict;
        private static readonly Vector3 FromSize = Vector3.one * 20f;
        private const float ToRadius = 10f;
        private const float MaxArrowLength = 15f;
#endif

        private readonly Dictionary<string, Component> _cacheDict = new();
        private readonly object _cacheDictLock = new();

        TComponent IComponentCache.GetComponent<TComponent>(
            GameObject findRoot,
            string path,
            bool includeInactive
        )
        {
            lock (_cacheDictLock)
            {
#if UNITY_EDITOR
                _updateCacheDict = true;
#endif
                return FinderUtil.GetComponent<TComponent>(findRoot, path, includeInactive, _cacheDict);
            }
        }

#if UNITY_EDITOR
        public Dictionary<int, Component[]> VisualizeCacheDictionary { get; private set; } = new();

        private void OnDrawGizmos()
        {
            if (_updateCacheDict)
            {
                lock (_cacheDictLock)
                {
                    VisualizeCacheDictionary = _cacheDict?.GroupBy(keyValuePair =>
                                                   {
                                                       var id = int.Parse(
                                                           keyValuePair.Key.Split('_').FirstOrDefault() ?? "0");
                                                       return id;
                                                   }, keyValuePair => keyValuePair.Value)
                                                   .ToDictionary(group => group.Key, group => group.ToArray()) ??
                                               new Dictionary<int, Component[]>();
                    _updateCacheDict = false;
                }
            }

            if (!visualize) return;
            var selectIds = UnityEditor.Selection.gameObjects.Select(go => go.GetInstanceID()).ToArray();
            foreach (var (id, components) in VisualizeCacheDictionary)
            {
                var obj = UnityEditor.EditorUtility.InstanceIDToObject(id);
                if (obj == null) continue;
                Gizmos.color = selectIds.Contains(id) ? selectedColor : defaultColor;
                var from = ((GameObject)obj).transform.position;

                Gizmos.DrawWireCube(from, FromSize);

                foreach (var component in components)
                {
                    if (component == null) continue;
                    var to = component.transform.position;
                    var direction = to - from;
                    if (direction == Vector3.zero)
                    {
                        Gizmos.DrawWireSphere(to, ToRadius);
                        continue;
                    }

                    Gizmos.DrawRay(from, direction);
                    var arrowLength = Mathf.Min(direction.magnitude / 10f, MaxArrowLength);
                    var lookRotation = Quaternion.LookRotation(direction);
                    var right = lookRotation * Quaternion.Euler(0, 180 + 20f, 0) *
                                new Vector3(0, 0, 1);
                    var left = lookRotation * Quaternion.Euler(0, 180 - 20f, 0) *
                               new Vector3(0, 0, 1);
                    Gizmos.DrawRay(to, right * arrowLength);
                    Gizmos.DrawRay(to, left * arrowLength);

                    Gizmos.DrawWireSphere(to, ToRadius);
                }
            }
        }
#endif

        private void OnDestroy()
        {
            lock (_cacheDictLock)
            {
#if UNITY_EDITOR
                _updateCacheDict = true;
#endif
                _cacheDict?.Clear();
            }

            RemoveComponentCache();
        }

        protected virtual void RemoveComponentCache()
        {
            Finder.RemoveLocalComponentCache(gameObject.GetInstanceID());
        }
    }
}