using System;
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
        public event Action<string> OnRun;

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
        }

        private void Run()
        {
            OnRun?.Invoke(PathField.text);
        }
    }
}