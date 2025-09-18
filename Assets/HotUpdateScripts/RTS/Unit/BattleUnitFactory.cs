using Rain.Core;
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
                case UnitType.Dummy:
                    unit = prefab.AddComponent<InfantryUnit>();
                    break;

                case UnitType.Archer:
                    unit = prefab.AddComponent<ArcherUnit>();
                    break;
                case UnitType.Cavalry:
                    unit = prefab.AddComponent<CavalryUnit>();
                    break;
                case UnitType.Mage:
                    unit = prefab.AddComponent<MageUnit>();
                    break;
                default:
                    Debug.LogError($"未知单位类型 {unitType}，使用默认步兵类型: {prefab.name}");
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
        public static BaseBattleUnit CreateUnitFromConfig(UnitConfig unitConfig, GameObject prefab, UnitInitData _initData = null)
        {
            BaseBattleUnit unit = CreateUnit(unitConfig.UnitType, prefab);

            if (unit != null)
            {
                unit.InitData(unitConfig, _initData);
                unit.Init();
            }

            return unit;
        }
    }
}
