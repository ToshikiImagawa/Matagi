# Matagi
This searches for a component within the descendants of a game object using a path.
And on subsequent calls, cached values will be retrieved.

Inspired by [ArrowCell](https://github.com/sassembla/ArrowCell).

# Required
- Unity 2021.3 or lator.
- .NET Standard 2.1.

# Install
1. Download com.comcreate-info.matagi-x.x.x.tgz [here](https://github.com/ToshikiImagawa/Matagi/releases).
2. [Install a package from a local tarball file](https://docs.unity3d.com/2021.3/Documentation/Manual/upm-ui-tarball.html).

# Quick start

## In the case you want to retrieve components of the same type distinguished by the GameObject's name.

Get the component attached to a GameObject with the same name as the property from its own descendants.

![Imgur](https://i.imgur.com/79jf3b3.png)
``` cs: View.cs
using Matagi;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    public Text Display => this.FindComponent<Text>(nameof(Display));
    public InputField PathField => this.FindComponent<InputField>(nameof(PathField));
    public Button RunButton => this.FindComponent<Button>(nameof(RunButton));
    public Button AddSceneButton => this.FindComponent<Button>(nameof(AddSceneButton));
    public Button CreateInstanceButton => this.FindComponent<Button>(nameof(CreateInstanceButton));
    public Button MoveSceneObjectButton => this.FindComponent<Button>(nameof(MoveSceneObjectButton));
}
```
