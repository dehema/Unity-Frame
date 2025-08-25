using System;
using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 投射物控制器，用于处理箭矢、魔法弹等投射物的行为
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        private BaseBattleUnit owner;        // 发射者
        private BaseBattleUnit target;       // 目标
        private int damage;              // 伤害值
        private float speed = 0f;       // 飞行速度
        private bool isInited = false;
        private float maxLifetime = 5f;  // 最大生命周期，防止永远不会销毁
        private float lifeTime = 0f;
        private float hitDistance = 0.5f; // 命中距离
        Action<GameObject> actionCollect;

        /// <summary>
        /// 初始化投射物
        /// </summary>
        /// <param name="_owner">发射者</param>
        /// <param name="_target">目标</param>
        /// <param name="damage">伤害值</param>
        /// <param name="speed">飞行速度</param>
        public void Init(BaseBattleUnit _owner, BaseBattleUnit _target, float _speed)
        {
            owner = _owner;
            target = _target;
            damage = _owner.Data.attack;
            speed = _speed;
            isInited = true;
            lifeTime = maxLifetime;
            transform.position = _owner.transform.position;
            transform.LookAt(_target.transform.position);
        }

        public void Update()
        {
            if (!isInited)
            {
                Collect();
                return;
            }

            // 更新生命周期
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Collect();
                return;
            }

            // 如果目标无效或已死亡，销毁投射物
            if (target == null || target.IsDead)
            {
                Collect();
                return;
            }

            // 移动到目标
            Vector3 targetPosition = target.transform.position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // 旋转投射物朝向目标
            transform.rotation = Quaternion.LookRotation(direction);

            // 检测是否击中目标
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (distanceToTarget < hitDistance)
            {
                // 造成伤害
                target.Hurt(damage);
                Debug.Log($"投射物击中 {target.UnitName}，造成 {damage} 点伤害");

                // 销毁投射物
                Collect();
            }
        }

        public void SetActionCollect(Action<GameObject> _action)
        {
            actionCollect = _action;
        }

        void Collect()
        {
            if (actionCollect == null)
            {
                Destroy(gameObject);
            }
            else
            {
                actionCollect.Invoke(gameObject);
            }
        }
    }
}
