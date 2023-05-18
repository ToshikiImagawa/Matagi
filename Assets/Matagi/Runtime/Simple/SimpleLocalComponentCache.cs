// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System.Collections.Generic;
using Matagi.Core;
using UnityEngine;

namespace Matagi.Simple
{
    [DisallowMultipleComponent]
    public class SimpleLocalComponentCache : MonoBehaviour, IComponentCache
    {
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
                return FinderUtil.GetComponent<TComponent>(
                    findRoot,
                    path,
                    includeInactive,
                    _cacheDict
                );
            }
        }

        private void OnDestroy()
        {
            lock (_cacheDictLock)
            {
                _cacheDict?.Clear();
            }
        }
    }
}