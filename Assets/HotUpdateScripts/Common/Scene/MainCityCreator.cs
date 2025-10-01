using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

/// <summary>
/// 主城创建器
/// </summary>
public class MainCityCreator : MonoBehaviour
{
    [SerializeField][Header("建筑父物体")] private GameObject buildingParent;

    private void Awake()
    {
        
    }

    public void OnDestroy()
    {
        
    }

    void OnSceneLoad(string sceneName)
    { 
    
    }
}
