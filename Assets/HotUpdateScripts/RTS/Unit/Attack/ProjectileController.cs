using System;
using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 投射物控制器，用于处理箭矢、魔法弹等投射物的行为
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ProjectileController : MonoBehaviour
    {
        private BaseBattleUnit owner;        // 发射者
        private BaseBattleUnit target;       // 目标
        private float speed = 15f;           // 飞行速度
        private bool isInited = false;
        private float maxLifetime = 5f;       // 最大生命周期，防止永远不会销毁
        private float lifeTime = 0f;

        [SerializeField] private float arcHeight = 2f;  // 抛物线高度

        private Vector3 startPos;            // 起始位置
        private Vector3 targetPos;           // 目标位置
        private float flightProgress = 0f;   // 飞行进度(0-1)


        private Action<GameObject> collectAction;    // 回收回调
        private Action<BaseBattleUnit> hitAction;    // 击中回调
        Rigidbody rigidbody;

        /// <summary>
        /// 初始化投射物
        /// </summary>
        /// <param name="_owner">发射者</param>
        /// <param name="_target">目标</param>
        /// <param name="_speed">飞行速度</param>
        public void Init(BaseBattleUnit _owner, BaseBattleUnit _target, float _speed)
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody>();
                if (rigidbody == null)
                {
                    rigidbody = gameObject.AddComponent<Rigidbody>();
                }
                rigidbody.useGravity = false;
            }

            owner = _owner;
            target = _target;
            speed = _speed;
            isInited = true;
            lifeTime = maxLifetime;

            // 记录起始位置和目标位置
            startPos = transform.position;
            targetPos = _target.transform.position;

            // 重置飞行进度
            flightProgress = 0f;

            // 初始朝向目标
            transform.LookAt(targetPos);
        }

        /// <summary>
        /// 设置回收回调
        /// </summary>
        /// <param name="action">回收回调函数</param>
        public void SetCollectAction(Action<GameObject> action)
        {
            collectAction = action;
        }

        public void SetHitAction(Action<BaseBattleUnit> action)
        {
            hitAction = action;
        }

        /// <summary>
        /// 回收投射物
        /// </summary>
        private void Collect()
        {
            isInited = false;

            if (collectAction != null)
            {
                collectAction(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 更新投射物位置和状态
        /// </summary>
        public void Update()
        {
            // 检查初始化状态
            if (!isInited)
            {
                Collect();
                return;
            }

            // 检查生命周期
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Collect();
                return;
            }

            // 检查目标是否有效
            if (target == null || target.IsDead)
            {
                Collect();
                return;
            }

            // 更新目标位置（目标可能在移动）
            targetPos = target.transform.position + new Vector3(0, target.Data.UnitConfig.height / 2, 0);

            // 计算抛物线轨迹
            UpdateParabolicTrajectory();
        }

        /// <summary>
        /// 计算并更新抛物线轨迹
        /// </summary>
        private void UpdateParabolicTrajectory()
        {
            // 增加飞行进度
            flightProgress += Time.deltaTime * speed / Vector3.Distance(startPos, targetPos);
            flightProgress = Mathf.Clamp01(flightProgress); // 确保值在0-1之间

            // 计算当前位置（线性插值 + 抛物线高度）
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, flightProgress);

            // 添加抛物线高度 (使用sin曲线模拟抛物线，在中间点达到最大高度)
            float heightOffset = Mathf.Sin(flightProgress * Mathf.PI) * arcHeight;
            currentPos.y += heightOffset;

            // 更新位置
            transform.position = currentPos;

            // 更新朝向（始终朝向飞行方向）
            if (flightProgress < 1.0f)
            {
                // 计算下一帧的位置来确定方向
                float nextProgress = Mathf.Clamp01(flightProgress + 0.01f);
                Vector3 nextPos = Vector3.Lerp(startPos, targetPos, nextProgress);
                nextPos.y += Mathf.Sin(nextProgress * Mathf.PI) * arcHeight;

                // 朝向下一个位置
                Vector3 direction = (nextPos - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isInited) return;
            // 检查是否碰到了目标单位
            BaseBattleUnit hitUnit = other.gameObject.GetComponent<BaseBattleUnit>();
            if (hitUnit != null)
            {
                // 敌人检测
                if (owner.IsEnemy(hitUnit))
                {
                    hitAction?.Invoke(hitUnit);
                }
            }
            Collect();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isInited) return;
            // 检查是否碰到了目标单位
            BaseBattleUnit hitUnit = collision.gameObject.GetComponent<BaseBattleUnit>();
            if (hitUnit != null)
            {
                // 敌人检测
                if (owner.IsEnemy(hitUnit))
                {
                    hitAction?.Invoke(hitUnit);
                }
            }
            Collect();
        }
    }
}
