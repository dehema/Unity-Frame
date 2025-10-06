using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

/// <summary>
/// ���Ǵ�����
/// </summary>
public class MainCityCreator : MonoBehaviour
{
    [SerializeField][Header("����������")] private Transform buildingParent;
    //������λ�ͽ���
    public Dictionary<int, GameObject> buildingSlot = new Dictionary<int, GameObject>();

    private void Awake()
    {
        InitAllBuilding();
    }

    void InitAllBuilding()
    {
        buildingSlot.Clear();
        foreach (var item in DataMgr.Ins.playerData.cityBuildings)
        {
            CityBuildingData cityBuildingData = item.Value;
            BuildingSlotConfig slotConfig = cityBuildingData.SlotConfig;
            CityBuildingConfig buildingConfig = cityBuildingData.BuildingConfig;
            GameObject preafab = Resources.Load<GameObject>(CityMgr.Ins.GetBuildingModelPath(buildingConfig.PlotModel));
            GameObject buildingGo = GameObject.Instantiate<GameObject>(preafab);
            buildingGo.transform.SetParent(buildingParent);
            buildingGo.transform.localPosition = new Vector3(slotConfig.PosX, slotConfig.PosY, slotConfig.PosZ);
            buildingGo.transform.eulerAngles = new Vector3(0, slotConfig.RotY, 0);
            buildingGo.transform.localScale = new Vector3(buildingConfig.Scale, buildingConfig.Scale, buildingConfig.Scale);
            buildingSlot[slotConfig.SlotID] = buildingGo;
            BuildingController buildingController = buildingGo.AddComponent<BuildingController>();
            buildingController.Init(cityBuildingData);
        }
    }
}
