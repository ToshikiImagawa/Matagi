// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System.Collections.Generic;
using Matagi.Core;
using UnityEngine;

namespace Matagi.Simple
{
    [DisallowMultipleComponent]
    public class SimpleLocalComponentCache : MonoBehaviour
    {
        private readonly Dictionary<string, Component> _cacheDict = new();
        private readonly object _cacheDictLock = new();

        internal TComponent GetComponent<TComponent>(
            GameObject findRoot,
            string path = null,
            bool includeInactive = false
        )
            where TComponent : Component
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