using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>整个地图数据</summary>
[Serializable]
public class SLGMapData
{
    public int sizeX;                                       // 地图尺寸
    public int sizeY;                                       // 地图尺寸
    public List<MapLayer> layers = new List<MapLayer>();    // 图层列表
}

/// <summary>单个图层数据</summary>
[Serializable]
public class MapLayer
{
    public int defaultTileIndex;                            //图层默认瓦片ID
    public string layerType;                                // 图层类型（Terrain/Resource/Obstacle 等）
    public string tileType;                                 // 瓦片标识类型（IntId/StringKey）
    public List<TileData> tiles = new List<TileData>();     // 瓦片列表（稀疏存储）
}

/// <summary>单个瓦片数据（仅含必要信息）</summary>
[Serializable]
public class TileData
{
    public int x;       // 网格X坐标（0~999）
    public int y;       // 网格Y坐标（0~999）
    public string id;   // 瓦片标识（兼容整数ID和字符串Key，统一用string存储）
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