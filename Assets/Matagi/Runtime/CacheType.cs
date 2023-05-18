namespace Matagi
{
    /// <summary>
    /// キャッシュの種類
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// 検索対象のRootのGameObjectにキャッシュします
        /// </summary>
        Local,

        /// <summary>
        /// シーンで１つのGameObjectにキャッシュします
        /// </summary>
        Scene,

        /// <summary>
        /// Staticにキャッシュします
        /// </summary>
        Static
    }
}