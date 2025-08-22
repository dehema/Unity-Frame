using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 战斗单位工厂类，用于创建不同类型的战斗单位
    /// </summary>
    public class BattleUnitFactory
    {
        /// <summary>
        /// 创建战斗单位
        /// </summary>
        /// <param name="unitType">单位类型</param>
        /// <param name="prefab">预制体</param>
        /// <returns>创建的战斗单位</returns>
        public static BaseBattleUnit CreateUnit(UnitType unitType, GameObject prefab)
        {
            // 移除可能已存在的战斗单位组件
            BaseBattleUnit existingUnit = prefab.GetComponent<BaseBattleUnit>();
            if (existingUnit != null)
            {
                GameObject.Destroy(existingUnit);
            }

            // 根据单位类型添加相应的组件
            BaseBattleUnit unit = null;

            switch (unitType)
            {
                case UnitType.Infantry:
                    unit = prefab.AddComponent<InfantryUnit>();
                    Debug.Log($"创建步兵单位: {prefab.name}");
                    break;

                case UnitType.Archer:
                    unit = prefab.AddComponent<ArcherUnit>();
                    Debug.Log($"创建弓箭手单位: {prefab.name}");
                    break;

                case UnitType.Cavalry:
                    // 骑兵单位可以在未来实现
                    Debug.LogWarning($"骑兵单位类型尚未实现，使用默认步兵类型: {prefab.name}");
                    unit = prefab.AddComponent<InfantryUnit>();
                    break;

                default:
                    Debug.LogWarning($"未知单位类型 {unitType}，使用默认步兵类型: {prefab.name}");
                    unit = prefab.AddComponent<InfantryUnit>();
                    break;
            }

            return unit;
        }

        /// <summary>
        /// 根据配置创建战斗单位
        /// </summary>
        /// <param name="unitConfig">单位配置</param>
        /// <param name="prefab">预制体</param>
        /// <returns>创建的战斗单位</returns>
        public static BaseBattleUnit CreateUnitFromConfig(UnitConfig unitConfig, GameObject prefab)
        {
            BaseBattleUnit unit = CreateUnit(unitConfig.unitType, prefab);

            if (unit != null)
            {
                unit.Init();
                unit.InitData(unitConfig);
            }

            return unit;
        }
    }
}
