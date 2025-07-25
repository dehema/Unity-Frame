﻿using System;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Rain.Core
{
    public class ResMgr : MonoSingleton<ResMgr>
    {
        public void CollectAnimation(Transform _startTf, ResType _resType, Action _cb = null)
        {
        }

        /// <summary>
        /// 增加奖励
        /// </summary>  
        /// <param name="_resType"></param>
        /// <param name="_num"></param>
        public void AddRes(ResType _resType, int _num)
        {
            DBObject dbObject = GetResFeild(_resType);
            if (dbObject != null)
            {
                DBInt dBFloat = (DBInt)dbObject;
                dBFloat.Value += _num;
                if (GameSettingStatic.ResLog)
                {
                    Debug.Log(string.Format("获得奖励{0}{1},总计{2}", _resType.ToString(), _num, dBFloat.Value));
                }
            }
        }

        /// <summary>
        /// 资源数量
        /// </summary>
        /// <param name="_resType"></param>
        public float GetResNum(ResType _resType)
        {
            DBObject dbObject = GetResFeild(_resType);
            return dbObject != null ? (float)dbObject.Value : 0f;
        }

        /// <summary>
        /// 资源数量
        /// </summary>
        /// <param name="_resType"></param>
        public DBObject GetResFeild(ResType _resType)
        {
            switch (_resType)
            {
                case ResType.gold:
                    return DataMgr.Ins.playerData.gold;
            }
            return null;
        }

        /// <summary>
        /// 获得资源图片
        /// </summary>
        /// <param name="_resType"></param>
        /// <returns></returns>
        public Sprite GetResSprite(ResType _resType)
        {
            string path = string.Empty;
            switch (_resType)
            {
                case ResType.gold:
                    path = "UI/UI_Gold";
                    break;
                case ResType.cash:
                    path = "UI/UI_Cash";
                    break;
                case ResType.amazon:
                    path = "UI/UI_Amazon";
                    break;
                case ResType.puzzle:
                    break;
            }
            if (!string.IsNullOrEmpty(path))
            {
                return Resources.Load<Sprite>(path);
            }
            return null;
        }
    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public enum ResType
    {
        gold,
        cash,
        amazon,
        puzzle
    }
}