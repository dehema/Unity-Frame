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
        private float speed = 15f;       // 飞行速度
        private bool isInitialized = false;
        private float maxLifetime = 5f;  // 最大生命周期，防止永远不会销毁
        private float lifetime = 0f;
        private float hitDistance = 0.5f; // 命中距离

        /// <summary>
        /// 初始化投射物
        /// </summary>
        /// <param name="owner">发射者</param>
        /// <param name="target">目标</param>
        /// <param name="damage">伤害值</param>
        /// <param name="speed">飞行速度</param>
        public void Initialize(BaseBattleUnit owner, BaseBattleUnit target, int damage, float speed = 15f)
        {
            this.owner = owner;
            this.target = target;
            this.damage = damage;
            this.speed = speed;
            this.isInitialized = true;
            this.lifetime = 0f;
        }

        public void Update()
        {
            if (!isInitialized)
            {
                Destroy(gameObject);
                return;
            }

            // 更新生命周期
            lifetime += Time.deltaTime;
            if (lifetime > maxLifetime)
            {
                Destroy(gameObject);
                return;
            }

            // 如果目标无效或已死亡，销毁投射物
            if (target == null || target.IsDead)
            {
                Destroy(gameObject);
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
                Destroy(gameObject);
            }
        }
    }
}
