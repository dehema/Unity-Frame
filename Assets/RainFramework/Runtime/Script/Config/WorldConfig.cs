using System.Collections.Generic;

public class WorldConfig : ConfigBase
{
    public Dictionary<WorldUnitType, WorldUnitConfig> Unit = new Dictionary<WorldUnitType, WorldUnitConfig>();
}

public class WorldUnitConfig
{
    public WorldUnitType ID;
    public float size;
}
