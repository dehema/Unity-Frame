using System;

namespace Rain.Core
{
    /// <summary>
    /// LateUpdateRefreshAttribute标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LateUpdateRefreshAttribute : Attribute
    {
        
    }
}