// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using Matagi.Simple;
using UnityEngine;

namespace SampleApp.Simple
{
    public class FinderSample : MonoBehaviour
    {
        [SerializeField] private GameObject findRootGameObject;
        [SerializeField] private View view;

        private void Awake()
        {
            view.RunButton.onClick.AddListener(Run);
        }

        private void OnDestroy()
        {
            if (view == null) return;
            view.RunButton.onClick.RemoveListener(Run);
        }

        private void Run()
        {
            var path = view.PathField.text;
            var target = findRootGameObject.FindComponent<Target>(path);
            view.Display.text = $"Call path: {path}, targetId = {target.GetId()}";
        }
    }
}