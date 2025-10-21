// code generation.

using System.Collections.Generic;

namespace Rain.Core
{
    /// <summary>
    /// 资源映射表
    /// </summary>
    public static class ResMap
    {
        /// <summary>
        /// 资源映射信息<assetName,ResMapping>
        /// </summary>
        public static Dictionary<string, ResMapping> ResMappings
        {
            get => _resMappings;
            set => _resMappings = value;
        }

        public static Dictionary<string, ResMapping> _resMappings = new Dictionary<string, ResMapping>();
    }

    /// <summary>
    /// 资源映射信息
    /// </summary>
    public class ResMapping
    {
        public string assetName;    // 资源在 AB 包内的名称（物理名称）
        public string logicPath;    // 业务层调用的路径（逻辑路径）
        public string abName;       // 所属 AB 包名称
    }
}