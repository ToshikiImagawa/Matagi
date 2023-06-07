using UnityEngine;

namespace Matagi
{
    [DisallowMultipleComponent]
    public class SceneComponentCache : LocalComponentCache
    {
        protected override void RemoveComponentCache()
        {
            Finder.RemoveSceneComponentCache(gameObject.scene.handle);
        }
    }
}