using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameSettingConfig : ConfigBase
{
    public Dictionary<string, SettingConfigItem> Common = new Dictionary<string, SettingConfigItem>();
}

public class SettingConfigItem
{
    public string ID;
    public string val;
}