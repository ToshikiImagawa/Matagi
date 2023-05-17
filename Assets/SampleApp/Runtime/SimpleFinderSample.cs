using Matagi.Simple;
using UnityEngine;

namespace SampleApp
{
    public class SimpleFinderSample : MonoBehaviour
    {
        [SerializeField] private GameObject findRootGameObject;
        [SerializeField] private View view;

        private void Awake()
        {
            view.OnRun += Run;
        }

        private void OnDestroy()
        {
            view.OnRun -= Run;
        }

        private void Run(string path)
        {
            var target = findRootGameObject.FindComponent<Target>(path);
            view.Show($"Call path: {path}, targetId = {target.GetId()}");
        }
    }
}