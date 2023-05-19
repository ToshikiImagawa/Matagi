namespace Matagi
{
    /// <summary>
    /// CacheType
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// The cache is disposed of based on the lifecycle of the GameObject at the root of the search.
        /// </summary>
        Local,

        /// <summary>
        /// It is cached on a single GameObject that exists only once in the scene.
        /// This allows centralized management of the cache regardless of the search location.
        /// The cache is disposed of when the scene is unloaded.
        /// </summary>
        Scene,

        /// <summary>
        /// It is cached in a static member variable.
        /// This allows the cache to be accessible even when the game object is moved to a different scene. 
        /// The cache is disposed of when any of the CacheClear-related methods are called.
        /// </summary>
        Static
    }
}