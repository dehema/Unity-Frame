using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using Rain.Core;
using Rain.UI;
using UnityEngine;

/// <summary>
/// CityHUDView
/// </summary>
public partial class CityHUDView : BaseView
{
    CityHUDViewParam param;
    MainCityCreator cityCreator => MainCityCreator.Ins;
    ObjPool poolBuildingMenu;
    Dictionary<BuildingController, BuildingHudItem> hudItems = new Dictionary<BuildingController, BuildingHudItem>();
    float nameFadeVal = 1;

    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        param = _viewParam as CityHUDViewParam;
        MsgMgr.Ins.AddEventListener(MsgEvent.CameraMove, OnCameraMove, cityCreator);
        MsgMgr.Ins.AddEventListener(MsgEvent.CameraZoomRatioChange, OnCameraScale, cityCreator);
        MsgMgr.Ins.AddEventListener(MsgEvent.SelectCityBuilding, OnSelectBuilding, cityCreator);
        InitBuildingMenus();
    }

    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
        ResetNameFadeVal();
    }

    private void InitBuildingMenus()
    {
        hudItems.Clear();
        poolBuildingMenu = PoolMgr.Ins.CreatePool(ui.buildingHudItem);
        foreach (var item in cityCreator.buildingSlots)
        {
            BuildingController buildingController = item.Value;
            BuildingHudItem buildingHudItem = poolBuildingMenu.Get<BuildingHudItem>(item.Value);
            hudItems[item.Value] = buildingHudItem;
        }
        UpdateBuildingMenusPos();
    }

    /// <summary>
    /// 更新菜单位置
    /// </summary>
    private void UpdateBuildingMenusPos()
    {
        foreach (var item in hudItems)
        {
            BuildingController buildingController = item.Key;
            BuildingHudItem buildingHudItem = item.Value;
            Vector3 screenPos = SceneMgr.Ins.Camera.WorldToScreenPoint(buildingController.transform.position);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, UIMgr.Ins.Camera, out Vector2 localPos))
            {
                buildingHudItem.rect.anchoredPosition = localPos;
            }
        }
    }

    private void UpdateBuildingNameFadeVal()
    {
        foreach (var item in hudItems)
        {
            BuildingHudItem buildingHudItem = item.Value;
            buildingHudItem.SetBuildingNameFadeVal(nameFadeVal);
        }
    }

    Tweener tweenerFade;
    /// <summary>
    /// 隐藏名字
    /// </summary>
    void ResetNameFadeVal()
    {
        tweenerFade?.Kill();
        nameFadeVal = 1;
        UpdateBuildingNameFadeVal();
        tweenerFade = DOTween.To(() => nameFadeVal, x => nameFadeVal = x, 0f, 1f).SetDelay(1).SetEase(Ease.OutQuad)
            .OnUpdate(UpdateBuildingNameFadeVal);
    }

    public void OnCameraMove(object[] param)
    {
        UpdateBuildingMenusPos();
        ResetNameFadeVal();
    }

    public void OnCameraScale(object[] param)
    {
        UpdateBuildingMenusPos();
    }

    private void OnSelectBuilding(object[] objs)
    {

    }
}

public class CityHUDViewParam : IViewParam
{

}
