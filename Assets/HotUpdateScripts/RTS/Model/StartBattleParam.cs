
using System.Collections.Generic;

/// <summary>
/// ��ʼս������
/// </summary>
public class StartBattleParam
{
    public List<StartBattleArmyParam> startBattleArmyParams = new List<StartBattleArmyParam>();
}

/// <summary>
/// ������Ϣ
/// </summary>
public class StartBattleArmyParam
{
    /// <summary>
    /// ��ϵ
    /// </summary>
    public Faction faction;
    /// <summary>
    /// �������ID
    /// </summary>
    public int formationID = 1;
    /// <summary>
    /// ��ҳ�ʼ����
    /// </summary>
    public Dictionary<int, int> initUnitNum = new Dictionary<int, int>();

    public StartBattleArmyParam(Faction faction)
    {
        this.faction = faction;
    }
}