using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>整个地图数据</summary>
[Serializable]
public class WorldMapConfig
{
    public Dictionary<int, MapLayer> layers = new Dictionary<int, MapLayer>();    // 图层列表
    public int sizeWidth;                                       // 地图尺寸
    public int sizeHeight;                                       // 地图尺寸
}

/// <summary>单个图层数据</summary>
[Serializable]
public class MapLayer
{
    public int defaultTileIndex = 4;                            //图层默认瓦片ID
    public Dictionary<int, TileData> tiles = new Dictionary<int, TileData>();     // 瓦片列表（稀疏存储）
}

/// <summary>单个瓦片数据（仅含必要信息）</summary>
[Serializable]
public class TileData
{
    public int index;   // 瓦片索引
    public int x;       // 网格X坐标
    public int y;       // 网格Y坐标

    public TileData() { }
    public TileData(int _index, int _x, int _y) { index = _index; x = _x; y = _y; }
}

/// <summary>
/// 瓦片类型
/// </summary>
public enum TileUnitType
{
    Tree = 0,       //树
    Water = 1,      //水
    Sea = 2,        //海洋
    mountain = 3,   //山
    plain = 4,      //草原
}