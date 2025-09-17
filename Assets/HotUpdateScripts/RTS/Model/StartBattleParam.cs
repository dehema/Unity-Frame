
using System.Collections.Generic;

/// <summary>
/// 开始战斗参数
/// </summary>
public class StartBattleParam
{
    public List<StartBattleArmyParam> startBattleArmyParams = new List<StartBattleArmyParam>();
}

/// <summary>
/// 军队信息
/// </summary>
public class StartBattleArmyParam
{
    /// <summary>
    /// 派系
    /// </summary>
    public Faction faction;
    /// <summary>
    /// 玩家阵型ID
    /// </summary>
    public int formationID = 1;
    /// <summary>
    /// 玩家初始部队
    /// </summary>
    public Dictionary<int, int> initUnitNum = new Dictionary<int, int>();

    public StartBattleArmyParam(Faction faction)
    {
        this.faction = faction;
    }
}