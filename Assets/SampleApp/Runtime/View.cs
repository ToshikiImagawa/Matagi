// Matagi C# reference source
// Copyright (c) 2023 COMCREATE. All rights reserved.

using Matagi;
using UnityEngine;
using UnityEngine.UI;

namespace SampleApp
{
    public class View : MonoBehaviour
    {
        public Text Display => this.FindComponent<Text>(nameof(Display), cacheType: CacheType.Local);
        public InputField PathField => this.FindComponent<InputField>(nameof(PathField), cacheType: CacheType.Local);
        public Button RunButton => this.FindComponent<Button>(nameof(RunButton), cacheType: CacheType.Local);
        public Button AddSceneButton => this.FindComponent<Button>(nameof(AddSceneButton), cacheType: CacheType.Local);

        public Button CreateInstanceButton =>
            this.FindComponent<Button>(nameof(CreateInstanceButton), cacheType: CacheType.Local);

        public Button MoveSceneObjectButton =>
            this.FindComponent<Button>(nameof(MoveSceneObjectButton), cacheType: CacheType.Local);
    }
}