using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMgr : MonoSingleton<PlayerMgr>
{
    /// <summary>
    /// ��ȡ�Ƽ�����
    /// </summary>
    /// <param name="_techID"></param>
    /// <returns></returns>
    public TechData GetTechData(string _techID)
    {
        //DataMgr.Ins.playerData.techs.TryGetValue(_techID, out TechData techData);
        //return techData;
        return new TechData();
    }

    /// <summary>
    /// ��ȡ�Ƽ�״̬
    /// </summary>
    /// <returns></returns>
    public TechState GetTechState(string _techID)
    {
        TechData techData = GetTechData(_techID);
        return (TechState)techData.state.Value;
    }
}
