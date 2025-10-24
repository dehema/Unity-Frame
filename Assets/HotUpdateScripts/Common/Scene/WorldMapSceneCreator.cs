using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

/// <summary>
/// 主城创建器
/// </summary>
public class WorldMapSceneCreator : MonoBehaviour
{

    public static WorldMapSceneCreator Ins;

    private void Awake()
    {
        Ins = this;
    }
}
