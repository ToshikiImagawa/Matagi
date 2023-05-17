// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Matagi
{
    [DisallowMultipleComponent]
    public sealed class LocalComponentCache : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector] public bool visualize;
        public Color defaultColor = Color.yellow;
        private bool _updateCacheDict;
        private static readonly Vector3 FromSize = Vector3.one * 20f;
        private const float ToRadius = 10f;
        private static readonly Color SelectedColor = Color.red;
#endif

        private readonly Dictionary<string, Component> _cacheDict = new();
        private readonly object _cacheDictLock = new();

        public TComponent GetComponent<TComponent>(GameObject findRoot, string path = null,
            bool includeInactive = false)
            where TComponent : Component
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
            foreach (var keyValuePair in VisualizeCacheDictionary)
            {
                var id = keyValuePair.Key;
                var obj = UnityEditor.EditorUtility.InstanceIDToObject(id);
                if (obj == null) continue;
                Gizmos.color = selectIds.Contains(id) ? SelectedColor : defaultColor;
                var from = ((GameObject)obj).transform.position;

                Gizmos.DrawWireCube(from, FromSize);

                foreach (var component in keyValuePair.Value)
                {
                    if (component == null) continue;
                    var to = component.transform.position;
                    var direction = to - from;
                    var arrowLength = direction.magnitude / 10f;
                    Gizmos.DrawRay(from, direction);
                    var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20f, 0) *
                                new Vector3(0, 0, 1);
                    var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20f, 0) *
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
        }
    }
}