using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace Rain.Core.RTS
{
    /// <summary>
    /// 战斗管理器单例
    /// </summary>
    public class BattleMgr : MonoSingleton<BattleMgr>
    {
        // 当前战斗状态
        public BattleState CurrentState { get; private set; }
        // 所有战斗单位列表
        private Dictionary<Faction, List<BattleUnit>> _factionUnits = new Dictionary<Faction, List<BattleUnit>>();

        private void Awake()
        {
            // 初始化阵营单位字典
            foreach (Faction faction in System.Enum.GetValues(typeof(Faction)))
            {
                _factionUnits[faction] = new List<BattleUnit>();
            }
            CurrentState = BattleState.Prepare;
        }

        // 注册单位
        public void RegisterUnit(BattleUnit unit)
        {
            if (!_factionUnits.ContainsKey(unit.Data.faction))
            {
                _factionUnits[unit.Data.faction] = new List<BattleUnit>();
            }

            if (!_factionUnits[unit.Data.faction].Contains(unit))
            {
                _factionUnits[unit.Data.faction].Add(unit);
            }

            CheckBattleState();
        }

        // 注销单位
        public void UnregisterUnit(BattleUnit unit)
        {
            if (_factionUnits.ContainsKey(unit.Data.faction))
            {
                _factionUnits[unit.Data.faction].Remove(unit);
            }

            CheckBattleState();
        }

        // 单位死亡时调用
        public void OnUnitDied(BattleUnit unit)
        {
            Debug.Log($"{unit.UnitName} 已被消灭");
            CheckBattleState();
        }

        // 获取特定阵营的存活单位
        public List<BattleUnit> GetAliveUnitsByFaction(Faction faction)
        {
            if (!_factionUnits.ContainsKey(faction))
                return new List<BattleUnit>();

            return _factionUnits[faction].FindAll(u => u.IsAlive());
        }

        /// <summary>
        /// 检查两个单位是否为敌对阵营
        /// </summary>
        /// <returns></returns>
        public bool isFactionEnemy(BattleUnit _battleUnit1, BattleUnit _battleUnit2)
        {
            Relation relation = _battleUnit1.GetFactionRelation(_battleUnit2.Data.faction);
            return relation == Relation.Hostile;
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

        // 开始战斗
        public void StartBattle()
        {
            CurrentState = BattleState.InProgress;
            Debug.Log("战斗开始！");
        }

        // 战斗结束处理
        private void OnBattleEnd(bool isVictory)
        {
            // 可以添加战斗结束逻辑，如显示结算界面等
        }

        public void AllUnitMove(Vector3 targetPos)
        {
            UnitMove(_factionUnits[Faction.Player], targetPos);
            Debug.Log("玩家部队 开始向坐标移动: " + targetPos);
        }

        // 向单位下达移动命令
        public void UnitMove(List<BattleUnit> units, Vector3 targetPos)
        {
            foreach (var unit in units)
            {
                if (unit.IsAlive())
                {
                    if (unit.stateMachine.currentState.stateType != UnitStateType.Move)
                    {
                        unit.SetMoveTarget(targetPos);
                        unit.stateMachine.ChangeState(new MoveState());
                    }
                    else
                    {
                        MoveState moveState = unit.stateMachine.currentState as MoveState;
                        moveState.MoveTo(targetPos);
                    }
                }
            }
        }

        // 向单位下达攻击命令
        public void FactionUnitAttackUnit(Faction _faction, BattleUnit target)
        {
            List<BattleUnit> units = _factionUnits[_faction];
            foreach (var unit in units)
            {
                if (unit.IsAlive() && unit.IsEnemy(target))
                {
                    unit.ClearMoveTarget();
                    unit.SetAttackTarget(target);

                    if (unit.IsTargetInAttackRange())
                    {
                        unit.stateMachine.ChangeState(new AttackState());
                    }
                    else
                    {
                        unit.stateMachine.ChangeState(new MoveState());
                    }
                }
            }
        }


        /// <summary>
        /// 检查两个单位是否为敌对关系
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEnemy(BattleUnit _unit1, BattleUnit _unit2)
        {
            bool res = GetFactionRelation(_unit1.Data.faction, _unit2.Data.faction) == Relation.Hostile;
            return res;
        }

        /// <summary>
        /// 获取阵营关系
        /// </summary>
        /// <param name="ownFaction"></param>
        /// <param name="_faction2"></param>
        /// <returns></returns>
        public Relation GetFactionRelation(Faction _faction1, Faction _faction2)
        {
            // 相同阵营为友好
            if (_faction1 == _faction2)
                return Relation.Friendly;

            if (_faction1 == Faction.Dummy || _faction2 == Faction.Dummy)
                return Relation.Hostile;

            // 玩家与敌人、怪物为敌对
            if ((_faction1 == Faction.Player && _faction2 == Faction.Enemy) ||
                (_faction1 == Faction.Enemy && _faction2 == Faction.Player))
                return Relation.Hostile;

            // 玩家与友方为友好
            if ((_faction1 == Faction.Player && _faction2 == Faction.Ally) ||
                (_faction1 == Faction.Ally && _faction2 == Faction.Player))
                return Relation.Friendly;

            // 其他情况为中立
            return Relation.Neutral;
        }
    }
}