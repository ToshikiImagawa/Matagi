// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using Matagi;
using UnityEngine;
using UnityEngine.UI;

namespace SampleApp
{
    public class View : MonoBehaviour
    {
        public Text Display => this.FindComponent<Text>(nameof(Display));
        public InputField PathField => this.FindComponent<InputField>(nameof(PathField));
        public Button RunButton => this.FindComponent<Button>(nameof(RunButton));
    }
}