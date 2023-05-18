// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using UnityEngine;

namespace SampleApp
{
    public class FindRootGameObjectSetter : MonoBehaviour
    {
        private void Awake()
        {
            if (FinderSample.CurrentFinderSample == null) return;
            FinderSample.CurrentFinderSample.findRootGameObject = gameObject;
        }
    }
}