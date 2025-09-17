using System;
using System.Collections.Generic;
using System.Linq;
using Rain.Core;
using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 战斗数据 
    /// </summary>
    public class BattleData
    {
        /// <summary>
        /// 区域内单位数据
        /// </summary>
        public struct AreaUnitData
        {
            public Vector4 area;                        // 区域矩形 左上角起始点 长宽
            public List<int> units;                     // 区域内的单位ID列表
            public UnitType unitType;                   // 区域单位类型
        }

        /// <summary>
        /// 玩家阵营ID
        /// </summary>
        public int playerFormationID;
        /// <summary>
        /// 初始单位数量 <UnitID,UnitNum>
        /// </summary>
        public Dictionary<int, int> initUnitNum = new Dictionary<int, int>();

        /// <summary>
        /// 所有区域的单位数据
        /// </summary>
        public List<AreaUnitData> areaUnitDatas = new List<AreaUnitData>();

        /// <summary>
        /// 单位位置 <位置,兵种ID>
        /// </summary>
        public Dictionary<Vector2, int> unitsPos = new Dictionary<Vector2, int>();

        // 临时字典，用于处理过程中的数据
        private Dictionary<UnitType, List<int>> _tempUnitsByType = new Dictionary<UnitType, List<int>>();

        public BattleData()
        {
            initUnitNum[1101] = 10;
            initUnitNum[1201] = 10;
            initUnitNum[1301] = 10;
            initUnitNum[1401] = 10;

            //计算单位的平均属性
            //foreach (var item in initUnitNum)
            //{
            //    UnitConfig unitConfig = ConfigMgr.Unit.Get(item.Key);

            //}
        }

        /// <summary>
        /// 分析阵型
        /// </summary>
        public void AnalyseFormation()
        {
            // 清空现有数据
            areaUnitDatas.Clear();

            GameObject prefab = Resources.Load<GameObject>($"Prefab/Formation/{playerFormationID}");
            if (prefab == null)
            {
                Debug.LogError($"无法加载阵型预制体: {playerFormationID}");
                return;
            }

            foreach (Transform item in prefab.transform)
            {
                string rootName = item.name;
                Debug.Log(rootName);

                if (Enum.TryParse(rootName, out UnitType unitType))
                {
                    RectTransform rectTransform = item.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        // 创建新的区域单位数据
                        AreaUnitData areaData = new AreaUnitData
                        {
                            area = new Vector4(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, rectTransform.rect.width, rectTransform.rect.height),
                            unitType = unitType,
                            units = new List<int>() // 初始化空列表，稍后会填充
                        };

                        // 添加到列表
                        areaUnitDatas.Add(areaData);
                    }
                    else
                    {
                        Debug.LogWarning($"阵型元素 {rootName} 没有RectTransform组件");
                    }
                }
                else
                {
                    Debug.LogWarning($"无法将 {rootName} 解析为单位类型");
                }
            }

            Debug.Log($"分析阵型完成，找到 {areaUnitDatas.Count} 个部署区域");
        }

        /// <summary>
        /// 分配单位类型
        /// </summary>
        public void DistributeUnitsUnitItem()
        {
            // 清空临时字典
            _tempUnitsByType.Clear();

            // 根据初始单位数量分类收集各类型单位
            foreach (var item in initUnitNum)
            {
                int unitID = item.Key;
                int count = item.Value;

                // 获取单位配置
                UnitConfig unitConfig = ConfigMgr.Unit.Get(unitID);

                // 根据单位类型分组
                if (!_tempUnitsByType.TryGetValue(unitConfig.UnitType, out List<int> unitList))
                {
                    unitList = new List<int>();
                    _tempUnitsByType[unitConfig.UnitType] = unitList;
                }

                // 添加指定数量的单位
                unitList.AddRange(Enumerable.Repeat(unitID, count));
            }

            Debug.Log($"分类完成，共有 {_tempUnitsByType.Count} 种单位类型");

            // 检查是否有类型没有对应的区域
            foreach (var unitType in _tempUnitsByType.Keys)
            {
                bool hasArea = areaUnitDatas.Any(area => area.unitType == unitType);
                if (!hasArea)
                {
                    Debug.LogWarning($"单位类型 {unitType} 没有对应的部署区域");
                }
            }
        }

        /// <summary>
        /// 单位分配到区域内
        /// </summary>
        public void DistributeUnitToArea()
        {
            // 逐个处理每种单位类型
            foreach (var unitTypeEntry in _tempUnitsByType)
            {
                UnitType unitType = unitTypeEntry.Key;
                List<int> unitIDs = unitTypeEntry.Value;

                // 找出该类型的所有区域
                var areasForType = areaUnitDatas.FindAll(area => area.unitType == unitType);

                if (areasForType.Count == 0)
                {
                    Debug.LogError($"没有找到类型 {unitType} 的部署区域");
                    continue;
                }

                // 计算每个区域应该分配的单位数量
                int unitsPerArea = unitIDs.Count / areasForType.Count;
                int remainingUnits = unitIDs.Count % areasForType.Count;

                // 分配单位到每个区域
                int unitIndex = 0;

                // 创建一个新的区域列表，因为我们需要修改原始列表中的元素
                List<AreaUnitData> updatedAreas = new List<AreaUnitData>();

                for (int areaIndex = 0; areaIndex < areasForType.Count; areaIndex++)
                {
                    // 获取当前区域数据
                    AreaUnitData areaData = areasForType[areaIndex];

                    // 确定当前区域应分配的单位数量
                    int unitsForThisArea = unitsPerArea;
                    if (remainingUnits > 0)
                    {
                        unitsForThisArea++;
                        remainingUnits--;
                    }

                    // 为当前区域分配单位
                    List<int> unitsForArea = new List<int>();
                    for (int i = 0; i < unitsForThisArea && unitIndex < unitIDs.Count; i++)
                    {
                        unitsForArea.Add(unitIDs[unitIndex]);
                        unitIndex++;
                    }

                    // 更新区域数据的单位列表
                    AreaUnitData updatedArea = areaData;
                    updatedArea.units = unitsForArea;
                    updatedAreas.Add(updatedArea);

                    Debug.Log($"区域 {areaData.area} 分配了 {unitsForArea.Count} 个 {unitType} 类型的单位");
                }

                // 更新原始列表中的区域数据
                for (int i = 0; i < areaUnitDatas.Count; i++)
                {
                    if (areaUnitDatas[i].unitType == unitType)
                    {
                        int index = areasForType.FindIndex(a => a.area == areaUnitDatas[i].area);
                        if (index >= 0 && index < updatedAreas.Count)
                        {
                            areaUnitDatas[i] = updatedAreas[index];
                        }
                    }
                }
            }

            // 清空临时字典，释放内存
            _tempUnitsByType.Clear();

            Debug.Log($"单位分配完成，共有 {areaUnitDatas.Count} 个区域被分配了单位");
        }
        
        /// <summary>
        /// 分配单位在区域内的具体位置（网格形式）
        /// </summary>
        /// <param name="unitSpacing">单位之间的最小间距（如果为0则自动计算）</param>
        public void DistributeUnitPositions(float unitSpacing = 0)
        {
            // 清空现有位置数据
            unitsPos.Clear();
            
            // 如果没有指定间距，计算一个合理的默认值
            if (unitSpacing <= 0)
            {
                unitSpacing = 1.0f; // 默认间距
            }
            
            // 遍历每个区域
            foreach (var areaData in areaUnitDatas)
            {
                // 跳过没有单位的区域
                if (areaData.units == null || areaData.units.Count == 0)
                    continue;
                
                // 获取区域信息
                float areaX = areaData.area.x;        // 区域左上角X坐标
                float areaY = areaData.area.y;        // 区域左上角Y坐标
                float areaWidth = areaData.area.z;    // 区域宽度
                float areaHeight = areaData.area.w;   // 区域高度
                
                // 计算可以放置的最大单位数量（网格）
                int unitCount = areaData.units.Count;
                
                // 计算网格行列数（尽量接近正方形网格）
                int cols = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
                int rows = Mathf.CeilToInt((float)unitCount / cols);
                
                // 计算实际使用的间距，使单位均匀分布在区域内
                float effectiveWidth = areaWidth - unitSpacing;
                float effectiveHeight = areaHeight - unitSpacing;
                
                float xSpacing = cols > 1 ? effectiveWidth / (cols - 1) : 0;
                float ySpacing = rows > 1 ? effectiveHeight / (rows - 1) : 0;
                
                // 如果只有一行或一列，则居中放置
                float xOffset = cols > 1 ? 0 : effectiveWidth / 2;
                float yOffset = rows > 1 ? 0 : effectiveHeight / 2;
                
                // 分配单位位置
                int index = 0;
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (index >= unitCount)
                            break;
                            
                        // 计算单位位置（相对于区域左上角）
                        float x = areaX + xOffset + col * xSpacing;
                        float y = areaY + yOffset + row * ySpacing;
                        
                        // 存储单位位置
                        Vector2 position = new Vector2(x, y);
                        unitsPos[position] = areaData.units[index];
                        
                        index++;
                    }
                }
                
                Debug.Log($"区域 {areaData.area} 分配了 {unitCount} 个单位位置");
            }
            
            // 调整相邻区域间的单位间隔
            AdjustBorderUnitSpacing();
            
            Debug.Log($"单位位置分配完成，共有 {unitsPos.Count} 个单位被分配了位置");
        }
        
        /// <summary>
        /// 调整相邻区域边界处的单位间隔
        /// </summary>
        private void AdjustBorderUnitSpacing()
        {
            // 获取所有区域的边界单位
            Dictionary<UnitType, List<KeyValuePair<Vector2, int>>> borderUnits = new Dictionary<UnitType, List<KeyValuePair<Vector2, int>>>();
            
            // 遍历每个区域，找出边界单位
            foreach (var areaData in areaUnitDatas)
            {
                if (areaData.units == null || areaData.units.Count == 0)
                    continue;
                    
                // 获取该区域的所有单位位置
                var areaUnitPositions = unitsPos.Where(p => areaData.units.Contains(p.Value)).ToList();
                
                // 找出边界单位（最左、最右、最上、最下的单位）
                if (areaUnitPositions.Count > 0)
                {
                    // 如果该类型还没有记录，创建新列表
                    if (!borderUnits.ContainsKey(areaData.unitType))
                    {
                        borderUnits[areaData.unitType] = new List<KeyValuePair<Vector2, int>>();
                    }
                    
                    // 添加边界单位
                    var leftMost = areaUnitPositions.OrderBy(p => p.Key.x).First();
                    var rightMost = areaUnitPositions.OrderByDescending(p => p.Key.x).First();
                    var topMost = areaUnitPositions.OrderBy(p => p.Key.y).First();
                    var bottomMost = areaUnitPositions.OrderByDescending(p => p.Key.y).First();
                    
                    borderUnits[areaData.unitType].Add(leftMost);
                    if (leftMost.Key != rightMost.Key) borderUnits[areaData.unitType].Add(rightMost);
                    if (leftMost.Key != topMost.Key && rightMost.Key != topMost.Key) borderUnits[areaData.unitType].Add(topMost);
                    if (leftMost.Key != bottomMost.Key && rightMost.Key != bottomMost.Key && topMost.Key != bottomMost.Key) 
                        borderUnits[areaData.unitType].Add(bottomMost);
                }
            }
            
            // 对于每种单位类型，调整相邻区域间的单位间隔
            foreach (var unitType in borderUnits.Keys)
            {
                var typeBorderUnits = borderUnits[unitType];
                
                // 如果只有一个区域，无需调整
                if (typeBorderUnits.Count <= 1)
                    continue;
                    
                // TODO: 实现更复杂的边界单位间隔调整算法
                // 当前简化版本只是确保边界单位不会过于拥挤
                const float minBorderSpacing = 1.5f; // 边界单位最小间距
                
                // 检查边界单位间距
                for (int i = 0; i < typeBorderUnits.Count; i++)
                {
                    for (int j = i + 1; j < typeBorderUnits.Count; j++)
                    {
                        Vector2 pos1 = typeBorderUnits[i].Key;
                        Vector2 pos2 = typeBorderUnits[j].Key;
                        
                        // 计算距离
                        float distance = Vector2.Distance(pos1, pos2);
                        
                        // 如果距离小于最小间距，调整位置
                        if (distance < minBorderSpacing)
                        {
                            // 计算移动方向
                            Vector2 direction = (pos2 - pos1).normalized;
                            
                            // 计算需要移动的距离
                            float moveDistance = (minBorderSpacing - distance) / 2;
                            
                            // 移动两个单位
                            Vector2 newPos1 = pos1 - direction * moveDistance;
                            Vector2 newPos2 = pos2 + direction * moveDistance;
                            
                            // 更新位置
                            int unitId1 = typeBorderUnits[i].Value;
                            int unitId2 = typeBorderUnits[j].Value;
                            
                            unitsPos.Remove(pos1);
                            unitsPos.Remove(pos2);
                            
                            unitsPos[newPos1] = unitId1;
                            unitsPos[newPos2] = unitId2;
                        }
                    }
                }
            }
        }
    }
}