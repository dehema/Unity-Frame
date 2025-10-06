using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMgr : MonoSingleton<PlayerMgr>
{
    /// <summary>
    /// 获取科技数据
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
    /// 获取科技状态
    /// </summary>
    /// <returns></returns>
    public TechState GetTechState(string _techID)
    {
        TechData techData = GetTechData(_techID);
        return (TechState)techData.state.Value;
    }
}
