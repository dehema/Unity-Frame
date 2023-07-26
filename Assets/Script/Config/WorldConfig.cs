using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WorldConfig : ConfigBase
{
    public Dictionary<WorldUnitType, WorldUnitConfig> Unit = new Dictionary<WorldUnitType, WorldUnitConfig>();
}

public class WorldUnitConfig
{
    public WorldUnitType ID;
    public float size;
}