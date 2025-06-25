using System.Collections.Generic;

public class GameSettingConfig : ConfigBase
{
    public Dictionary<string, SettingConfigItem> Common = new Dictionary<string, SettingConfigItem>();
}

public class SettingConfigItem
{
    public string ID;
    public string val;
}
