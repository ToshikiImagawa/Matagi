using System;
using System.Collections.Generic;
using UnityEngine;
using Matagi;
using UnityEngine.UI;

namespace SampleApp
{
    public class View : MonoBehaviour
    {
        private Text Display => this.FindComponent<Text>(nameof(Display));
        private InputField PathField => this.FindComponent<InputField>(nameof(PathField));
        private Button RunButton => this.FindComponent<Button>(nameof(RunButton));

        private readonly Subject _onRun = new Subject();
        public IObservable<string> OnRun => _onRun;

        public void Show(string text)
        {
            Display.text = text;
        }

        private void Awake()
        {
            RunButton.onClick.AddListener(Run);
        }

        private void OnDestroy()
        {
            RunButton.onClick.RemoveListener(Run);
            _onRun.OnCompleted();
            _onRun.Dispose();
        }

        private void Run()
        {
            _onRun.OnNext(PathField.text);
        }

        private class Subject : IObservable<string>, IDisposable
        {
            private bool _isDisposed;
            private bool _isCompleted;
            private readonly List<IObserver<string>> _observers = new();

            public void OnNext(string value)
            {
                if (_isCompleted) return;
                foreach (var observer in _observers)
                {
                    observer.OnNext(value);
                }
            }

            public void OnCompleted()
            {
                if (_isCompleted) return;
                _isCompleted = true;
                var observers = _observers.ToArray();
                _observers.Clear();
                foreach (var observer in observers)
                {
                    observer.OnCompleted();
                }
            }

            public IDisposable Subscribe(IObserver<string> observer)
            {
                if (_observers.Contains(observer)) return EmptyDisposable.Instance;
                _observers.Add(observer);
                return new Subscriber(this, observer);
            }

            public void Dispose()
            {
                if (_isDisposed) return;
                _isDisposed = true;
                _isCompleted = true;
                _observers.Clear();
            }

            private class Subscriber : IDisposable
            {
                private readonly Subject _subject;
                private readonly IObserver<string> _observer;
                private bool _isDisposed;

                public Subscriber(Subject subject, IObserver<string> observer)
                {
                    _subject = subject;
                    _observer = observer;
                }

                public void Dispose()
                {
                    if (_subject._isDisposed || _isDisposed) return;
                    _isDisposed = true;
                    if (_subject._observers.Contains(_observer))
                    {
                        _subject._observers.Remove(_observer);
                    }
                }
            }

            private class EmptyDisposable : IDisposable
            {
                public static EmptyDisposable Instance = new();

                public void Dispose()
                {
                }
            }
        }
    }
}