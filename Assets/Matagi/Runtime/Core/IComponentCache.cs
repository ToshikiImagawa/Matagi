using UnityEngine;

namespace Matagi.Core
{
    public interface IComponentCache
    {
        TComponent GetComponent<TComponent>(
            GameObject findRoot,
            string path = null,
            bool includeInactive = false
        ) where TComponent : Component;
    }
}