using System;
using System.Collections.Generic;

[System.Serializable]
public class PackageConfigData
{
    // 基本信息
    public string name;
    public string displayName;
    public string version;
    public string description;
    public string unity;
    public string documentationUrl;
    public string changelogUrl;
    public string licensesUrl;
    
    // 作者信息
    [System.Serializable]
    public class Author
    {
        public string name;
        public string email;
        public string url;
    }
    public Author author;
    
    // 依赖项
    [System.Serializable]
    public class Dependencies : Dictionary<string, string> { }
    public Dependencies dependencies;
    
    // 关键字
    public List<string> keywords;
    
    // 示例
    [System.Serializable]
    public class Sample
    {
        public string displayName;
        public string description;
        public string path;
    }
    
    [System.Serializable]
    public class Samples
    {
        public List<Sample> samples;
    }
    public Samples samples;
}