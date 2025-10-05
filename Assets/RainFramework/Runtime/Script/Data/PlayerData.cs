using System.Collections.Generic;
using Rain.Core;

public class PlayerData : DBClass
{
    //属性
    public DBString playerName;
    public DBInt level;
    public DBInt exp;

    public DBFloat hp;
    public DBFloat hpMax;

    //资源
    public DBLong gold;
    public DBLong wood;
    public DBLong diamond;

    //科技
    public Dictionary<string, TechData> techs = new Dictionary<string, TechData>();

    //建筑
    public Dictionary<int, CityBuildingData> cityBuildings = new Dictionary<int, CityBuildingData>();

    public PlayerData()
    {
        playerName = new DBString();
        level = new DBInt();
        exp = new DBInt();

        hp = new DBFloat();
        hpMax = new DBFloat();

        //资源
        gold = new DBLong();
        wood = new DBLong();
        diamond = new DBLong();
    }
}
