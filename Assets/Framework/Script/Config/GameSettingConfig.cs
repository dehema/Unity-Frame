using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameSettingConfig : ConfigBase
{
    public Dictionary<string, SettingConfig> Setting = new Dictionary<string, SettingConfig>();
}

public class SettingConfig
{
    public string key;
    public string val;
}