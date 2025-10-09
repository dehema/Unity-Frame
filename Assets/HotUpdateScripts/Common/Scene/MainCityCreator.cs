using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

/// <summary>
/// ���Ǵ�����
/// </summary>
public class MainCityCreator : MonoBehaviour
{
    public static MainCityCreator Ins;
    [SerializeField][Header("����������")] private Transform buildingParent;
    //������λ�ͽ���
    public Dictionary<int, BuildingController> buildingSlots { get; private set; }

    private void Awake()
    {
        Ins = this;
        InitAllBuilding();
    }

    void InitAllBuilding()
    {
        buildingSlots = new Dictionary<int, BuildingController>();
        foreach (var item in DataMgr.Ins.playerData.cityBuildings)
        {
            CityBuildingData cityBuildingData = item.Value;
            BuildingSlotConfig slotConfig = cityBuildingData.SlotConfig;
            CityBuildingConfig buildingConfig = cityBuildingData.BuildingConfig;
            GameObject buildingGo = new GameObject();
            buildingGo.name = buildingConfig.BuildingType.ToString(); ;
            buildingGo.transform.SetParent(buildingParent);
            buildingGo.transform.localPosition = new Vector3(slotConfig.PosX, slotConfig.PosY, slotConfig.PosZ);
            buildingGo.transform.eulerAngles = new Vector3(0, slotConfig.RotY, 0);
            buildingGo.transform.localScale = new Vector3(buildingConfig.Scale, buildingConfig.Scale, buildingConfig.Scale);
            BuildingController buildingController = buildingGo.AddComponent<BuildingController>();
            buildingController.Init(cityBuildingData);
            buildingSlots[slotConfig.SlotID] = buildingController;
        }
    }
}
