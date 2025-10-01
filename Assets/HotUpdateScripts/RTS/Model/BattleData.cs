using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rain.Core;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 战斗数据 
    /// </summary>
    public class BattleData
    {
        // 战斗场景
        public BattleType battleType;
        // 所有参战军队数据
        public List<BattleArmyData> battleArmyDatas = new List<BattleArmyData>();

        //军队单位坐标缩放系数
        private const float armyScaleFactor = 20;

        public BattleData()
        {

        }

        public BattleData(StartBattleParam startBattleParam)
        {
            foreach (StartBattleArmyParam param in startBattleParam.startBattleArmyParams)
            {
                BattleArmyData battleArmyData = new BattleArmyData(param);
                battleArmyDatas.Add(battleArmyData);
            }
        }

        /// <summary>
        /// 设置军队朝向并重新计算位置
        /// </summary>
        /// <param name="faction">军队阵营</param>
        /// <param name="rotation">朝向角度（Y轴旋转）</param>
        public void SetArmyRotation(Faction faction, float rotation)
        {
            var army = battleArmyDatas.FirstOrDefault(a => a.faction == faction);
            if (army != null)
            {
                army.spawnRot = rotation;
                // 重新计算该军队的位置（应用旋转）
                AnalyseArmy(army);
            }
        }

        /// <summary>
        /// 分析单位位置
        /// </summary>
        public void AnalyseAllArmy()
        {
            foreach (var army in battleArmyDatas)
            {
                AnalyseArmy(army);
            }
        }

        public void AnalyseArmy(BattleArmyData army)
        {
            AnalyseFormation(army);
            DistributeUnitsToAreas(army);
            DistributeUnitPositions(army);
        }

        /// <summary>
        /// 分析阵型
        /// </summary>
        private void AnalyseFormation(BattleArmyData army)
        {
            // 清空现有数据
            army.areaUnitDatas.Clear();

            GameObject prefab = Resources.Load<GameObject>($"Prefab/Formation/{army.formationID}");
            if (prefab == null)
            {
                Debug.LogError($"无法加载阵型预制体: {army.formationID}");
                return;
            }

            foreach (Transform item in prefab.transform)
            {
                string rootName = item.name;
                //Debug.Log(rootName);

                if (Enum.TryParse(rootName, out UnitType unitType))
                {
                    RectTransform rectTransform = item.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        // 创建新的区域单位数据
                        AreaUnitData areaData = new AreaUnitData
                        {
                            // 起始点在队伍的左上方
                            area = new Vector4(rectTransform.anchoredPosition.x - rectTransform.rect.width / 2
                            , rectTransform.anchoredPosition.y - rectTransform.rect.height / 2,
                            rectTransform.rect.width, rectTransform.rect.height),
                            unitType = unitType,
                            units = new List<int>() // 初始化空列表，稍后会填充
                        };

                        // 添加到列表
                        army.areaUnitDatas.Add(areaData);
                    }
                    else
                    {
                        Debug.LogError($"阵型元素 {rootName} 没有RectTransform组件");
                    }
                }
                else
                {
                    Debug.LogError($"无法将 {rootName} 解析为单位类型");
                }
            }

            //Debug.Log($"分析阵型完成，找到 {army.areaUnitDatas.Count} 个部署区域");
        }

        /// <summary>
        /// 直接分配单位到对应区域
        /// </summary>
        private void DistributeUnitsToAreas(BattleArmyData army)
        {
            // 按单位类型分组处理初始单位数据
            var unitTypeGroups = army.initUnitNum
                .GroupBy(kv => ConfigMgr.Unit.Get(kv.Key).UnitType)
                .ToList();

            foreach (var typeGroup in unitTypeGroups)
            {
                UnitType unitType = typeGroup.Key;

                // 收集该类型的所有单位ID（展开数量）
                List<int> unitIDs = new List<int>();
                foreach (var unitEntry in typeGroup)
                {
                    int unitID = unitEntry.Key;
                    int count = unitEntry.Value;
                    unitIDs.AddRange(Enumerable.Repeat(unitID, count));
                }

                // 找出该类型的所有区域
                var areasForType = army.areaUnitDatas
                    .Select((area, index) => new { area, index })
                    .Where(x => x.area.unitType == unitType)
                    .ToList();

                if (areasForType.Count == 0)
                {
                    Debug.LogError($"单位类型 {unitType} 没有对应的部署区域");
                    continue;
                }

                // 计算每个区域应该分配的单位数量
                int unitsPerArea = unitIDs.Count / areasForType.Count;
                int remainingUnits = unitIDs.Count % areasForType.Count;

                // 直接分配单位到区域
                int unitIndex = 0;
                for (int areaIdx = 0; areaIdx < areasForType.Count; areaIdx++)
                {
                    var areaInfo = areasForType[areaIdx];
                    int originalIndex = areaInfo.index;

                    // 确定当前区域应分配的单位数量
                    int unitsForThisArea = unitsPerArea + (areaIdx < remainingUnits ? 1 : 0);

                    // 为当前区域分配单位
                    List<int> unitsForArea = new List<int>();
                    for (int i = 0; i < unitsForThisArea && unitIndex < unitIDs.Count; i++)
                    {
                        unitsForArea.Add(unitIDs[unitIndex]);
                        unitIndex++;
                    }

                    // 直接更新原始区域数据
                    AreaUnitData updatedArea = army.areaUnitDatas[originalIndex];
                    updatedArea.units = unitsForArea;
                    army.areaUnitDatas[originalIndex] = updatedArea;

                    //Debug.Log($"区域 {updatedArea.area} 分配了 {unitsForArea.Count} 个 {unitType} 类型的单位");
                }
            }

            //Debug.Log($"单位分配完成，共有 {army.areaUnitDatas.Count} 个区域被分配了单位");
        }

        /// <summary>
        /// 分配单位在区域内的具体位置（优化的对称布局）
        /// 确保军队中心在spawnPos，整体朝向为spawnRot
        /// </summary>
        private void DistributeUnitPositions(BattleArmyData army)
        {
            // 清空现有位置数据
            army.unitsPos.Clear();

            // 遍历每个区域，生成单位位置
            foreach (var areaData in army.areaUnitDatas)
            {
                if (areaData.units == null || areaData.units.Count == 0)
                    continue;

                // 获取区域信息
                float areaX = areaData.area.x;
                float areaY = areaData.area.y;
                float areaWidth = areaData.area.z;
                float areaHeight = areaData.area.w;

                int total = areaData.units.Count;

                // 基于区域宽高比计算最优行列数，确保布局合理
                float aspect = areaWidth / Mathf.Max(0.0001f, areaHeight);
                int cols = Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(total * aspect)));
                int rows = Mathf.Max(1, Mathf.CeilToInt((float)total / cols));

                // 计算每一行的Y位置，在区域高度内等距分布，避免贴边且上下对称
                List<float> rowYPositions = new List<float>();
                float rowStep = areaHeight / (rows + 1);
                for (int r = 0; r < rows; r++)
                {
                    float y = areaY + rowStep * (r + 1);
                    rowYPositions.Add(y);
                }

                // 计算各行应分配的单位数量，使各行数量差不超过1
                int basePerRow = total / rows;
                int remainder = total % rows;
                int[] rowCounts = new int[rows];
                for (int r = 0; r < rows; r++)
                {
                    rowCounts[r] = basePerRow + (r < remainder ? 1 : 0);
                }

                // 按行布置单位
                int idx = 0;
                for (int r = 0; r < rows; r++)
                {
                    if (idx >= total) break;
                    int countInRow = rowCounts[r];

                    // 在本行内等分宽度，避免贴边且左右对称
                    float xStep = areaWidth / (countInRow + 1);
                    float y = rowYPositions[Mathf.Min(r, rowYPositions.Count - 1)];

                    for (int c = 0; c < countInRow; c++)
                    {
                        int unitId = areaData.units[idx++];
                        float x = areaX + xStep * (c + 1);

                        // 计算相对于阵型中心的位置
                        Vector2 localPos = new Vector2(x, y);

                        // 应用军队旋转
                        Vector2 rotatedPos = RotatePosition(localPos, army.spawnRot);

                        // 平移到最终的世界位置（以spawnPos为中心）
                        Vector2 finalPos = rotatedPos / armyScaleFactor + new Vector2(army.spawnPos.x, army.spawnPos.z);

                        army.unitsPos[finalPos] = unitId;
                    }
                }

                //Debug.Log($"区域 {areaData.area} 布置了 {total} 个单位，rows={rows}, cols={cols}");
            }

            //Debug.Log($"单位位置分配完成，共有 {army.unitsPos.Count} 个单位被分配了位置，中心位置: {army.spawnPos}");
        }

        /// <summary>
        /// 旋转位置坐标
        /// </summary>
        /// <param name="position">原始位置</param>
        /// <param name="rotationY">Y轴旋转角度（度）</param>
        /// <returns>旋转后的位置</returns>
        private Vector2 RotatePosition(Vector2 position, float rotationY)
        {
            if (Mathf.Abs(rotationY) < 0.01f) return position; // 没有旋转

            float radians = rotationY * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);

            float newX = position.x * cos - position.y * sin;
            float newY = position.x * sin + position.y * cos;

            return new Vector2(newX, newY);
        }
    }

    /// <summary>
    /// 区域内单位数据
    /// </summary>
    public struct AreaUnitData
    {
        public Vector4 area;                        // 区域矩形 左上角起始点 长宽
        public List<int> units;                     // 区域内的单位ID列表
        public UnitType unitType;                   // 区域单位类型
    }

    public class BattleArmyData
    {
        // 阵营ID
        public Faction faction;

        // 阵型ID
        public int formationID;

        //军队出生点
        public Vector3 spawnPos;

        // 军队朝向（Y轴旋转角度）
        public float spawnRot = 0f;

        // 初始单位数量 <UnitID,UnitNum>
        public Dictionary<int, int> initUnitNum = new Dictionary<int, int>();

        // 所有区域的单位数据
        public List<AreaUnitData> areaUnitDatas = new List<AreaUnitData>();

        // 单位位置 <位置,兵种ID>
        public Dictionary<Vector2, int> unitsPos = new Dictionary<Vector2, int>();

        public BattleArmyData(StartBattleArmyParam param)
        {
            this.faction = param.faction;
            this.formationID = param.formationID;
            this.initUnitNum = param.initUnitNum;
        }
    }
}