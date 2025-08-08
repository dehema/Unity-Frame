using System;
using System.Collections.Generic;

public class AllUnitConfig : ConfigBase
{
    public List<UnitConfig> unit = new List<UnitConfig>();

    public override void OnLoadComplete()
    {
        foreach (var item in unit)
        {
            item.unitType = (UnitType)Enum.Parse(typeof(UnitType), item._unitType);
        }
    }
}

public class UnitConfig
{
    /// <summary>
    /// ID      
    /// </summary>
    public int ID;
    public string _unitType;
    /// <summary>
    /// 单位类型
    /// </summary>
    public UnitType unitType = UnitType.Infantry;
    /// <summary>
    /// 阵营
    /// </summary>
    public Faction faction = Faction.Player;
    public string name;
    /// <summary>
    /// 生命值
    /// </summary>
    public int hp;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// 攻击力
    /// </summary>
    public int attack;
    /// <summary>
    /// 攻击间隔
    /// </summary>
    public float attackInterval;
    /// <summary>
    /// 攻击范围
    /// </summary>
    public float attackRange;
    /// <summary>
    /// 攻击敌人后,对方多久受伤
    /// </summary>
    public float attackHurtTime;
    /// <summary>
    /// 攻击时长
    /// </summary>
    public float attackDuration;
    /// <summary>
    /// 转向速度
    /// </summary>
    public float angularSpeed;
    /// <summary>
    /// 模型高度
    /// </summary>
    public float height;
    /// <summary>
    /// 价值/招募价格
    /// </summary>
    public int value;
    /// <summary>
    /// 每级价值增长
    /// </summary>
    public int value_pre;
    /// <summary>
    /// 维护费
    /// </summary>
    public int upkeep;
    public string fullID;

}

public enum UnitType
{
    /// <summary>
    /// 假人
    /// </summary>
    Dummy,
    /// <summary>
    /// 玩家
    /// </summary>
    Player = -1,
    /// <summary>
    /// 步兵
    /// </summary>
    Infantry = 1,
    /// <summary>
    /// 弓箭手
    /// </summary>
    Archer,
    /// <summary>
    /// 骑兵
    /// </summary>
    Cavalry
}

public enum Faction
{
    /// <summary>
    /// 玩家阵营
    /// </summary>
    Player,
    /// <summary>
    /// 敌人阵营
    /// </summary>
    Enemy,
    /// <summary>
    /// 中立阵营
    /// </summary>
    Neutral,
    /// <summary>
    /// 盟友阵营
    /// </summary>
    Ally,
    /// <summary>
    /// 假人
    /// </summary>
    Dummy,
}