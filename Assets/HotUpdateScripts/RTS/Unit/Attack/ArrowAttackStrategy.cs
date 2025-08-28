using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 弓箭攻击策略，适用于弓箭手等远程单位
    /// </summary>
    public class ArrowAttackStrategy : IAttackStrategy
    {
        private float speed = 20f;
        public static ObjPool arrowPool;
        BaseBattleUnit attacker;

        public ArrowAttackStrategy()
        {
        }

        public void SetProjectilePrefabPath(string _arrowPath)
        {
            if (arrowPool != null)
            {
                arrowPool.Clear();
            }
            else
            {
                //PoolMgr.Ins.DestroyPool(arrowPool.id);
                arrowPool = PoolMgr.Ins.CreatePoolFromPath(_arrowPath);
            }
        }


        public void Attack(BaseBattleUnit _attacker, BaseBattleUnit _target, Vector3 _shootPos)
        {
            if (_target == null || _target.IsDead)
                return;
            if (arrowPool == null)
                return;

            attacker = _attacker;
            GameObject projectileObj = arrowPool.Get();

            ProjectileController projectile = projectileObj.GetComponent<ProjectileController>();
            projectileObj.transform.position = _attacker.bodyPart.shootPos.transform.position;
            if (projectile == null)
            {
                projectile = projectileObj.AddComponent<ProjectileController>();
            }
            projectile.SetCollectAction(SetCollectAction);
            projectile.SetHitAction(SetHitAction);
            projectile.Init(_attacker, _target, speed);
            Debug.Log($"[{_attacker.UnitName}] 向 [{_target.UnitName}] 射箭");
        }

        public void SetCollectAction(GameObject _go)
        {
            arrowPool.CollectOne(_go);
        }

        public void SetHitAction(BaseBattleUnit _unit)
        {
            _unit.Hurt(attacker.Data.attack);
            Debug.Log($"[{attacker.UnitName}] 的箭命中了 [{_unit.UnitName}] ,造成 [{attacker.Data.attack}] 点伤害");
        }
    }
}
