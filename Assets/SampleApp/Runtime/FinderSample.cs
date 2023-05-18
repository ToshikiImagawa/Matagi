// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using Matagi;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SampleApp
{
    public class FinderSample : MonoBehaviour
    {
        public GameObject findRootGameObject;
        [SerializeField] private View view;
        [SerializeField] private GameObject prefab;

        public static FinderSample CurrentFinderSample { get; private set; }

        private void Awake()
        {
            CurrentFinderSample = this;
            view.RunButton.onClick.AddListener(Run);
            view.AddSceneButton.onClick.AddListener(AddScene);
            view.CreateInstanceButton.onClick.AddListener(CreateInstance);
        }

        private void OnDestroy()
        {
            CurrentFinderSample = null;
            if (view == null) return;
            view.RunButton.onClick.RemoveListener(Run);
            view.AddSceneButton.onClick.RemoveListener(AddScene);
            view.CreateInstanceButton.onClick.RemoveListener(CreateInstance);
        }

        private void Run()
        {
            var path = view.PathField.text;
            Finder.DefaultCacheType = CacheType.Scene;
            var target = findRootGameObject.FindComponent<Target>(path);
            view.Display.text = $"Call path: {path}, targetId = {target.GetId()}";
            Finder.DefaultCacheType = CacheType.Local;
        }

        private void AddScene()
        {
            SceneManager.LoadScene("AdditionalScene", LoadSceneMode.Additive);
        }

        private void CreateInstance()
        {
            Instantiate(prefab);
        }
    }
}