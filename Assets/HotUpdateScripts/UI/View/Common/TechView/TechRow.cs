using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

public partial class TechRow : InfiniteScrollItem
{
    TechRowData techRowData;
    static ObjPool techRootPool = null;
    public List<GameObject> techRootGos = new List<GameObject>();

    public override void SetActive(bool active, bool notifyEvent = true)
    {
        base.SetActive(active, notifyEvent);
        InitPool();
    }

    void InitPool()
    {
        if (techRootPool != null)
            return;
        TechView techView = UIMgr.Ins.GetView<TechView>();
        GameObject go = Instantiate(AssetMgr.Ins.Load<GameObject>("TechRoot"), UIMgr.Ins.GetView<TechView>().transform);
        if (go)
        {
            techRootPool = PoolMgr.Ins.CreatePool(go, techView.gameObject);
        }
    }

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);
        techRowData = scrollData as TechRowData;
        ui.topLine.SetActive(techRowData.index != 1);
        //回收旧科技
        foreach (var item in techRootGos)
        {
            techRootPool.CollectOne(item);
        }
        techRootGos.Clear();
        //生成新科技
        foreach (TechConfig config in techRowData.techConfigs)
        {
            GameObject go = techRootPool.Get();
            techRootGos.Add(go);
            go.transform.SetParent(ui.roots.transform);
            TechRoot.TechRootData techRootData = new TechRoot.TechRootData();
            techRootData.techConfig = config;
            techRootData.techCategoryConfig = techRowData.techCategoryConfig;
            go.GetComponent<TechRoot>().Init(techRootData);
        }
    }

    /// <summary>
    /// 科技行数据
    /// </summary>
    public class TechRowData : UIControlDemo_DynamicContainerItemData
    {

        public List<TechConfig> techConfigs = new List<TechConfig>();   //科技配置
        public TechCategoryConfig techCategoryConfig;                   //科技分类
    }
}