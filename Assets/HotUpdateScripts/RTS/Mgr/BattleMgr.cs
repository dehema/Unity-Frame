using System.Collections.Generic;
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

        // 向单位下达移动命令
        public void IssueMoveOrder(List<BattleUnit> units, Vector3 targetPosition)
        {
            foreach (var unit in units)
            {
                if (unit.IsAlive())
                {
                    unit.SetMoveTarget(targetPosition);
                    if (unit.stateMachine.CurrentState is not MoveState &&
                        unit.stateMachine.CurrentState is not AttackState)
                    {
                        unit.stateMachine.ChangeState(new MoveState());
                    }
                }
            }
        }

        // 向单位下达攻击命令
        public void IssueAttackOrder(List<BattleUnit> units, BattleUnit target)
        {
            foreach (var unit in units)
            {
                if (unit.IsAlive() && unit.IsEnemy(target))
                {
                    unit.SetAttackTarget(target);
                    unit.ClearMoveTarget();

                    if (unit.IsTargetInAttackRange())
                    {
                        unit.stateMachine.ChangeState(new AttackState());
                    }
                    else
                    {
                        unit.stateMachine.ChangeState(new IdleState());
                    }
                }
            }
        }
    }
}