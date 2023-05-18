// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using UnityEngine;

namespace SampleApp
{
    [DisallowMultipleComponent]
    public class Target : MonoBehaviour
    {
        [SerializeField] private string id;

        public string GetId()
        {
            return id;
        }
    }
}