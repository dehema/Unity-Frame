using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>������ͼ����</summary>
[Serializable]
public class WorldMapConfig
{
    public Dictionary<int, MapLayer> layers = new Dictionary<int, MapLayer>();    // ͼ���б�
    public int sizeWidth;                                       // ��ͼ�ߴ�
    public int sizeHeight;                                       // ��ͼ�ߴ�
}

/// <summary>����ͼ������</summary>
[Serializable]
public class MapLayer
{
    public int defaultTileIndex = 4;                            //ͼ��Ĭ����ƬID
    public Dictionary<int, TileData> tiles = new Dictionary<int, TileData>();     // ��Ƭ�б�ϡ��洢��
}

/// <summary>������Ƭ���ݣ�������Ҫ��Ϣ��</summary>
[Serializable]
public class TileData
{
    public int index;   // ��Ƭ����
    public int x;       // ����X����
    public int y;       // ����Y����

    public TileData() { }
    public TileData(int _index, int _x, int _y) { index = _index; x = _x; y = _y; }
}

/// <summary>
/// ��Ƭ����
/// </summary>
public enum TileUnitType
{
    Tree = 0,       //��
    Water = 1,      //ˮ
    Sea = 2,        //����
    mountain = 3,   //ɽ
    plain = 4,      //��ԭ
}