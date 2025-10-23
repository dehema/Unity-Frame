using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>������ͼ����</summary>
[Serializable]
public class SLGMapData
{
    public int sizeX;                                       // ��ͼ�ߴ�
    public int sizeY;                                       // ��ͼ�ߴ�
    public List<MapLayer> layers = new List<MapLayer>();    // ͼ���б�
}

/// <summary>����ͼ������</summary>
[Serializable]
public class MapLayer
{
    public int defaultTileIndex;                            //ͼ��Ĭ����ƬID
    public string layerType;                                // ͼ�����ͣ�Terrain/Resource/Obstacle �ȣ�
    public string tileType;                                 // ��Ƭ��ʶ���ͣ�IntId/StringKey��
    public List<TileData> tiles = new List<TileData>();     // ��Ƭ�б�ϡ��洢��
}

/// <summary>������Ƭ���ݣ�������Ҫ��Ϣ��</summary>
[Serializable]
public class TileData
{
    public int x;       // ����X���꣨0~999��
    public int y;       // ����Y���꣨0~999��
    public string id;   // ��Ƭ��ʶ����������ID���ַ���Key��ͳһ��string�洢��
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