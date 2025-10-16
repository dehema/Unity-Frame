using System;

namespace Rain.Core
{
    /// <summary>
    /// UpdateRefreshAttribute标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UpdateRefreshAttribute : Attribute
    {
        
    }
}