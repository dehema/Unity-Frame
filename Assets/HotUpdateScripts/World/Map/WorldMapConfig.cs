using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>整个地图数据</summary>
[Serializable]
public class WorldMapConfig
{
    public int MapSize;                                                             // 每个区域的尺寸
    public int SizePerArea;                                                         // 单方向上的地图数量
    public Dictionary<int, MapLayer> Layers = new Dictionary<int, MapLayer>();      // 图层列表
    public Dictionary<int, TileArea> Areas = new Dictionary<int, TileArea>();       // 图层列表
}

/// <summary>单个图层数据</summary>
[Serializable]
public class MapLayer
{
    public Dictionary<int, int> Areas = new Dictionary<int, int>();   //区域数据
}

/// <summary>单个瓦片数据（仅含必要信息）</summary>
[Serializable]
public class TileArea
{
    public int DefaultTile = 4;
    public Dictionary<int, TileData> Tiles = new Dictionary<int, TileData>();   // 瓦片列表（稀疏存储）
}

/// <summary>单个瓦片数据</summary>
[Serializable]
public class TileData
{
    public int Index;   // 瓦片索引(区域相对索引，不能直接使用)
    public int Type;    // 瓦片索引
    [NonSerialized]
    public Vector2Int Pos;

    public TileData() { }
    public TileData(int _index) { Index = _index; }
    public TileData(TileData _otherData)
    {
        this.Index = _otherData.Index;
        this.Type = _otherData.Type;
        this.Pos = _otherData.Pos;
    }
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