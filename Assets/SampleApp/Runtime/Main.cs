using System;
using Matagi.Simple;
using UnityEngine;

namespace SampleApp
{
    public class Main : MonoBehaviour
    {
        [SerializeField] private GameObject findRootGameObject;
        [SerializeField] private View view;

        private IDisposable _disposable;

        private void Awake()
        {
            _disposable = view.OnRun.Subscribe(new Observer(Run));
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        private void Run(string path)
        {
            var target = findRootGameObject.FindComponent<Target>(path);
            view.Show($"Call path: {path}, targetId = {target.GetId()}");
        }

        private class Observer : IObserver<string>
        {
            private readonly Action<string> _onNext;

            public Observer(Action<string> onNext)
            {
                _onNext = onNext;
            }

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(string value)
            {
                _onNext.Invoke(value);
            }
        }
    }
}