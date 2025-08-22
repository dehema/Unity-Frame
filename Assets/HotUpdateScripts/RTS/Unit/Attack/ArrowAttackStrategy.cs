using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 弓箭攻击策略，适用于弓箭手等远程单位
    /// </summary>
    public class ArrowAttackStrategy : IAttackStrategy
    {
        private float projectileSpeed = 15f;
        public static ObjPool arrowPool;

        public ArrowAttackStrategy()
        {
            // 如果没有提供预制体，尝试加载默认的
            //this.projectilePrefab = projectilePrefab ?? Resources.Load<GameObject>(arrowPrefabPath);
            //if (this.projectilePrefab == null)
            //{
            //    Debug.LogWarning("无法加载投射物预制体，将使用直接伤害模式");
            //}
            //arrowPool = PoolMgr.Ins.CreatePool()
        }

        public void SetProjectilePrefabPath(string _arrowPath)
        {
            if (arrowPool != null)
            {
                arrowPool.Clear();
            }
            //arrowPool = PoolMgr.Ins.CreatePool(_arrowPath);
        }


        public void PerformAttack(BaseBattleUnit attacker, BaseBattleUnit target)
        {
            if (target == null || target.IsDead)
                return;

            // 检查是否有射击位置
            if (attacker.shootPos == null)
            {
                Debug.LogWarning($"{attacker.UnitName} 没有设置射击位置 (shootPos)，无法正确发射投射物");
                // 退化为直接伤害
                target.Hurt(attacker.Data.attack);
                return;
            }

            // 如果有预制体，创建投射物
            if (arrowPrefab == null)
                return;
            GameObject projectileObj = GameObject.Instantiate(arrowPrefab, attacker.shootPos.transform.position, Quaternion.identity);

            ProjectileController projectile = projectileObj.GetComponent<ProjectileController>();
            if (projectile == null)
            {
                projectile = projectileObj.AddComponent<ProjectileController>();
            }

            projectile.Initialize(attacker, target, attacker.Data.attack, projectileSpeed);
            Debug.Log($"{attacker.UnitName} 向 {target.UnitName} 发射了一个投射物");
        }
    }
}
