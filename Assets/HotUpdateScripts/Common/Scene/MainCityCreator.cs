using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

/// <summary>
/// 主城创建器
/// </summary>
public class MainCityCreator : MonoBehaviour
{
    [SerializeField][Header("建筑父物体")] private Transform buildingParent;
    //建筑栏位和建筑
    public Dictionary<int, GameObject> buildingSlot = new Dictionary<int, GameObject>();

    private void Awake()
    {
        InitAllBuilding();

    }


    void InitAllBuilding()
    {
        buildingSlot.Clear();
        foreach (BuildingSlotConfig slotConfig in ConfigMgr.CityBuildingSlot.DataList)
        {
            CityBuildingConfig buildingConfig = ConfigMgr.CityBuilding.DataMap[slotConfig.BuildingType];
            GameObject preafab = Resources.Load<GameObject>(CityMgr.Ins.GetBuildingModelPath(buildingConfig.PlotModel));
            GameObject buildingGo = GameObject.Instantiate<GameObject>(preafab);
            buildingGo.transform.SetParent(buildingParent);
            buildingGo.transform.localPosition = new Vector3(slotConfig.PosX, slotConfig.PosY, slotConfig.PosZ);
            buildingGo.transform.eulerAngles = new Vector3(0, slotConfig.RotY, 0);
            buildingGo.transform.localScale = new Vector3(buildingConfig.Scale, buildingConfig.Scale, buildingConfig.Scale);
            buildingSlot[slotConfig.SlotID] = buildingGo;
            buildingGo.AddComponent<BuildingController>();
        }
    }
}
