// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using System.Collections;
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
            Finder.DefaultCacheType = CacheType.Scene;
            CurrentFinderSample = this;
            view.RunButton.onClick.AddListener(Run);
            view.AddSceneButton.onClick.AddListener(AddScene);
            view.CreateInstanceButton.onClick.AddListener(CreateInstance);
            view.MoveSceneObjectButton.onClick.AddListener(MoveSceneObject);
        }

        private void OnDestroy()
        {
            CurrentFinderSample = null;
            if (view == null) return;
            view.RunButton.onClick.RemoveListener(Run);
            view.AddSceneButton.onClick.RemoveListener(AddScene);
            view.CreateInstanceButton.onClick.RemoveListener(CreateInstance);
            view.MoveSceneObjectButton.onClick.RemoveListener(MoveSceneObject);
        }

        private void Run()
        {
            var path = view.PathField.text;
            var target = findRootGameObject.FindComponent<Target>(path);
            view.Display.text = $"Call path: {path}, targetId = {target.GetId()}";
        }

        private void AddScene()
        {
            Finder.DefaultCacheType = CacheType.Scene;
            SceneManager.LoadScene("AdditionalScene", LoadSceneMode.Additive);
        }

        private void CreateInstance()
        {
            Finder.DefaultCacheType = CacheType.Scene;
            Instantiate(prefab);
        }

        private void MoveSceneObject()
        {
            Finder.DefaultCacheType = CacheType.Static;
            StartCoroutine(RemoveScene());

            IEnumerator RemoveScene()
            {
                yield return SceneManager.LoadSceneAsync("MoveScene", LoadSceneMode.Additive);
                Run();
                SceneManager.MoveGameObjectToScene(findRootGameObject.gameObject,
                    SceneManager.GetSceneByName("FinderSampleScene"));
                yield return SceneManager.UnloadSceneAsync("MoveScene");
            }
        }
    }
}