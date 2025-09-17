using System.Collections.Generic;
using UnityEngine;
using Rain.Core;
using Rain.UI;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 战斗管理器单例
    /// </summary>
    public class BattleMgr : MonoSingleton<BattleMgr>
    {
        public Camera unitCamera;
        // 当前战斗状态
        public BattleState CurrentState { get; private set; }
        // 所有战斗单位列表
        private Dictionary<Faction, List<BaseBattleUnit>> _factionUnits = new Dictionary<Faction, List<BaseBattleUnit>>();

        public BattleData battleData;

        public void InitBattle(StartBattleParam _startBattleParam)
        {
            //处理数据
            // 初始化阵营单位字典
            foreach (Faction faction in System.Enum.GetValues(typeof(Faction)))
            {
                _factionUnits[faction] = new List<BaseBattleUnit>();
            }
            CurrentState = BattleState.Prepare;

            //本场战斗
            battleData = new BattleData(_startBattleParam);

            // 打开HUD
            UIMgr.Ins.OpenView(ViewName.RTSHudView);

            TimerMgr.Ins.AddTimer(this, delay: 0.5f, onComplete: () =>
            {
                //处理战场数据
                foreach (var army in battleData.battleArmyDatas)
                {
                    Transform trans = null;
                    if (army.faction == Faction.Player)
                    {
                        trans = RTSUnitSceneMgr.Ins.sceneRoot.playerBirthPos?.transform;
                    }
                    else if (army.faction == Faction.Enemy)
                    {
                        trans = RTSUnitSceneMgr.Ins.sceneRoot.enemyBirthPos?.transform;
                    }
                    Vector3 spawnPos = trans?.position ?? Vector3.zero;
                    Vector3 spawnRot = trans?.eulerAngles ?? Vector3.zero;
                    army.spawnPos = spawnPos;
                    army.spawnRot = spawnRot.y;
                }
                battleData.AnalyseAllArmy();

                // 按军队数据生成单位
                foreach (var army in battleData.battleArmyDatas)
                {
                    foreach (var kv in army.unitsPos)
                    {
                        Vector2 pos = kv.Key;
                        int unitID = kv.Value;
                        // 位置已经包含了旋转和平移，直接使用，只需要缩放和设置Y坐标
                        Vector3 worldPos = new Vector3(pos.x, 0, pos.y);
                        BaseBattleUnit battleUnit = CreateUnit(unitID, worldPos, new Vector3(0, army.spawnRot, 0));
                        battleUnit.Data.faction = army.faction;
                    }
                }

                //战斗开始
                StartBattle();
            });
        }

        // 开始战斗
        public void StartBattle()
        {
            // 战斗开始
            CurrentState = BattleState.InProgress;
            Debug.Log("战斗开始！");
        }

        public BaseBattleUnit CreateUnit(int _unitID, Vector3 _pos, Vector3 _rot)
        {
            UnitConfig unitConfig = ConfigMgr.Unit.Get(_unitID);
            BaseBattleUnit battleUnit = CreateUnit(unitConfig);
            battleUnit.transform.position = _pos;
            battleUnit.transform.eulerAngles = _rot;
            return battleUnit;
        }

        /// <summary>
        /// 创建单位
        /// </summary>
        /// <param name="unitConfig"></param>
        public BaseBattleUnit CreateUnit(UnitConfig unitConfig)
        {
            BaseBattleUnit battleUnit = null;
            GameObject prefab = Resources.Load<GameObject>("Prefab/Unit/" + unitConfig.FullID);
            if (prefab != null)
            {
                // 实例化单位预制体
                GameObject item = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                battleUnit = BattleUnitFactory.CreateUnitFromConfig(unitConfig, item);
            }
            return battleUnit;
        }

        // 注册单位
        public void RegisterUnit(BaseBattleUnit unit)
        {
            if (!_factionUnits.ContainsKey(unit.Data.faction))
            {
                _factionUnits[unit.Data.faction] = new List<BaseBattleUnit>();
            }

            if (!_factionUnits[unit.Data.faction].Contains(unit))
            {
                _factionUnits[unit.Data.faction].Add(unit);
            }
            MsgMgr.Ins.DispatchEvent(MsgEvent.RTSBattleUnitAdd, unit);
            CheckBattleState();
        }

        // 注销单位
        public void UnregisterUnit(BaseBattleUnit unit)
        {
            if (_factionUnits.ContainsKey(unit.Data.faction))
            {
                _factionUnits[unit.Data.faction].Remove(unit);
            }
            MsgMgr.Ins.DispatchEvent(MsgEvent.RTSBattleUnitRemove, unit);
            CheckBattleState();
        }

        // 单位死亡时调用
        public void OnUnitDied(BaseBattleUnit unit)
        {
            Debug.Log($"{unit.UnitName} 已被消灭");
            CheckBattleState();
        }

        // 获取特定阵营的存活单位
        public List<BaseBattleUnit> GetAliveUnitsByFaction(Faction faction)
        {
            if (!_factionUnits.ContainsKey(faction))
                return new List<BaseBattleUnit>();

            return _factionUnits[faction].FindAll(u => u.IsAlive());
        }

        /// <summary>
        /// 检查两个单位是否为敌对阵营
        /// </summary>
        /// <returns></returns>
        public bool isFactionEnemy(BaseBattleUnit _battleUnit1, BaseBattleUnit _battleUnit2)
        {
            CampRelation relation = _battleUnit1.GetFactionRelation(_battleUnit2.Data.faction);
            return relation == CampRelation.Hostile;
        }

        // 检查战斗状态
        private void CheckBattleState()
        {
            if (CurrentState != BattleState.InProgress) return;

            // 检查玩家是否胜利（所有敌方单位被消灭）
            bool allEnemiesDead = GetAliveUnitsByFaction(Faction.Enemy).Count == 0;

            if (allEnemiesDead)
            {
                CurrentState = BattleState.Victory;
                Debug.Log("战斗胜利！");
                OnBattleEnd(true);
                return;
            }

            // 检查玩家是否失败（玩家单位全灭）
            bool playerUnitsDead = GetAliveUnitsByFaction(Faction.Player).Count == 0;
            if (playerUnitsDead)
            {
                CurrentState = BattleState.Defeat;
                Debug.Log("战斗失败！");
                OnBattleEnd(false);
            }
        }

        // 战斗结束处理
        private void OnBattleEnd(bool isVictory)
        {
            // 可以添加战斗结束逻辑，如显示结算界面等
        }

        public void AllPlayerUnitMove(Vector3 targetPos)
        {
            UnitMove(_factionUnits[Faction.Player], targetPos);
            Debug.Log($"{GetFactionName(Faction.Player)}部队开始向坐标{targetPos}移动");
        }

        /// <summary>
        /// 阵营名称
        /// </summary>
        /// <param name="_faction"></param>
        /// <returns></returns>
        public string GetFactionName(Faction _faction)
        {
            switch (_faction)
            {
                case Faction.Player:
                default:
                    return "玩家";
                case Faction.Enemy:
                    return "敌人";
                case Faction.Neutral:
                    return "中立";
                case Faction.Ally:
                    return "盟友";
                case Faction.Dummy:
                    return "标靶";
            }
        }

        // 向单位下达移动命令
        public void UnitMove(List<BaseBattleUnit> units, Vector3 targetPos)
        {
            foreach (var unit in units)
            {
                unit.UnitMoveAndChangeState(targetPos);
            }
        }

        // 向单位下达攻击命令
        public void FactionUnitAttackUnit(Faction _faction, BaseBattleUnit target)
        {
            List<BaseBattleUnit> units = _factionUnits[_faction];
            foreach (var unit in units)
            {
                unit.AttackUnitAndChangeState(target);
            }
        }


        /// <summary>
        /// 检查两个单位是否为敌对关系
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEnemy(BaseBattleUnit _unit1, BaseBattleUnit _unit2)
        {
            bool res = GetFactionRelation(_unit1.Data.faction, _unit2.Data.faction) == CampRelation.Hostile;
            return res;
        }

        /// <summary>
        /// 获取阵营关系
        /// </summary>
        /// <param name="ownFaction"></param>
        /// <param name="_faction2"></param>
        /// <returns></returns>
        public CampRelation GetFactionRelation(Faction _faction1, Faction _faction2)
        {
            // 相同阵营为友好
            if (_faction1 == _faction2)
                return CampRelation.Friendly;

            if (_faction1 == Faction.Dummy || _faction2 == Faction.Dummy)
                return CampRelation.Hostile;

            // 玩家与敌人、怪物为敌对
            if ((_faction1 == Faction.Player && _faction2 == Faction.Enemy) ||
                (_faction1 == Faction.Enemy && _faction2 == Faction.Player))
                return CampRelation.Hostile;

            // 玩家与友方为友好
            if ((_faction1 == Faction.Player && _faction2 == Faction.Ally) ||
                (_faction1 == Faction.Ally && _faction2 == Faction.Player))
                return CampRelation.Friendly;

            // 其他情况为中立
            return CampRelation.Neutral;
        }
    }
}