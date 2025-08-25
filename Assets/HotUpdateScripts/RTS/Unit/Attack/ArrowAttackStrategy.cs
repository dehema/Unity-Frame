using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 弓箭攻击策略，适用于弓箭手等远程单位
    /// </summary>
    public class ArrowAttackStrategy : IAttackStrategy
    {
        private float projectileSpeed = 1f;
        public static ObjPool arrowPool;

        public ArrowAttackStrategy()
        {
        }

        public void SetProjectilePrefabPath(string _arrowPath)
        {
            if (arrowPool != null)
            {
                arrowPool.Clear();
            }
            arrowPool = PoolMgr.Ins.CreatePoolFromPath(_arrowPath);
        }


        public void Attack(BaseBattleUnit _attacker, BaseBattleUnit _target, Vector3 _shootPos)
        {
            if (_target == null || _target.IsDead)
                return;
            if (arrowPool == null)
                return;

            GameObject projectileObj = arrowPool.Get();

            ProjectileController projectile = projectileObj.GetComponent<ProjectileController>();
            projectileObj.transform.position = _attacker.bodyPart.shootPos.transform.position;
            if (projectile == null)
            {
                projectile = projectileObj.AddComponent<ProjectileController>();
            }
            projectile.SetActionCollect((go) => { arrowPool.CollectOne(go); });
            projectile.Init(_attacker, _target, projectileSpeed);
            Debug.Log($"{_attacker.UnitName} 向 {_target.UnitName} 发射了一个投射物");
        }
    }
}
