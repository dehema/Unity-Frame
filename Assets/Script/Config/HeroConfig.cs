using System.Collections.Generic;

public partial class HeroConfig : ConfigBase
{
    public Dictionary<int, AllItemConfig> all = new Dictionary<int, AllItemConfig>();

    public Dictionary<string, SkillItemConfig> skill = new Dictionary<string, SkillItemConfig>();

    public override void Init()
    {
    
    }
}

public class AllItemConfig
{
    public int id;
    public int price;
    public string desc;
}

public class SkillItemConfig
{
    public string id;
    public int value;
    public string desc;
}

