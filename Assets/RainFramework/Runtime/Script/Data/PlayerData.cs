using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

public class PlayerData : DBClass
{
    //--------------------------------------------------属性--------------------------------------------------
    public DBString playerName;
    public DBInt level;
    public DBInt exp;
    public DBFloat hp;
    public DBFloat hpMax;

    //--------------------------------------------------资源--------------------------------------------------
    public DBLong food;
    public DBLong wood;
    public DBLong gold;
    public DBLong book;
    public DBLong ore;
    public DBLong diamond;

    //背包
    public DBDictClass<string, DBItemData> items = new DBDictClass<string, DBItemData>();

    //科技
    public Dictionary<string, TechData> techs = new Dictionary<string, TechData>();

    //建筑
    public DBDictClass<int, CityBuildingData> cityBuildings = new DBDictClass<int, CityBuildingData>();

    //--------------------------------------------------通用--------------------------------------------------
    //创建时间
    public long createTime;

    //--------------------------------------------------世界地图--------------------------------------------------
    public WorldMapData worldMapData = new WorldMapData();

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
