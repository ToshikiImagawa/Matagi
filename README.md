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

# Introduction

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

# Types of Caches

- Local
- Scene
- Static

## Local
It is the default cache type.
The cache is disposed of based on the lifecycle of the GameObject at the root of the search.

## Scene
It is cached on a single GameObject that exists only once in the scene.
This allows centralized management of the cache regardless of the search location.
The cache is disposed of when the scene is unloaded.

## Static
It is cached in a static member variable.
This allows the cache to be accessible even when the game object is moved to a different scene. 
The cache is disposed of when any of the CacheClear-related methods are called.

# ComponentCache
When choosing Local or Scene cache support, the following components exist in the scene:

- LocalComponentCache
- SceneComponentCache

By pressing the Visualize button, the relationship between the Finder and the Find target is displayed as arrows in the Scene View.