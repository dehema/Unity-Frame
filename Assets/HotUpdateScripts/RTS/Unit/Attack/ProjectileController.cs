using System;
using System.Threading;
using Rain.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 投射物控制器，用于处理箭矢、魔法弹等投射物的行为
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        private BaseBattleUnit owner;        // 发射者
        private BaseBattleUnit target;       // 目标
        private float speed = 0;           // 飞行速度
        private bool isInited = false;
        private float maxLifetime = 5f;       // 最大生命周期，防止永远不会销毁
        private float lifeTime = 0f;

        [SerializeField] private float arcHeight = 2f;  // 抛物线高度

        private Vector3 startPos;            // 起始位置
        private Vector3 targetPos;           // 目标位置
        private float flightProgress = 0f;   // 飞行进度(0-1)


        private Action<GameObject> collectTraceAction;      // 轨迹回收回调
        ObjPool hitEffectPool;                              // 命中特效对象池
        private Action<BaseBattleUnit> hitAction;           // 击中回调
        Rigidbody rigid;
        DireType direType = DireType.Follow;
        Collider collider;

        /// <summary>
        /// 初始化投射物
        /// </summary>
        /// <param name="_owner">发射者</param>
        /// <param name="_target">目标</param>
        /// <param name="_speed">飞行速度</param>
        public void Init(BaseBattleUnit _owner, BaseBattleUnit _target, float _speed)
        {
            if (rigid == null)
            {
                rigid = GetComponent<Rigidbody>();

                if (rigid == null)
                {
                    rigid = gameObject.AddComponent<Rigidbody>();
                }
                rigid.useGravity = false;
                rigid.isKinematic = false; // 确保不是运动学刚体，以便能够触发碰撞
                rigid.collisionDetectionMode = CollisionDetectionMode.Continuous; // 使用连续碰撞检测，防止高速穿透
            }
            if (collider == null)
            {
                collider = GetComponent<Collider>();
            }

            owner = _owner;
            target = _target;
            speed = _speed;
            isInited = true;
            lifeTime = maxLifetime;

            // 记录起始位置和目标位置
            startPos = transform.position;
            targetPos = GetTargetPos();

            // 重置飞行进度
            flightProgress = 0f;

            // 初始朝向目标
            transform.LookAt(targetPos);

            //处理轨迹类型
            if (direType == DireType.Fixed)
            {
            }
            else if (direType == DireType.Follow)
            {
                if (rigid != null)
                {
                    Destroy(rigid);
                }
            }
            if (collider != null)
            {
                collider.enabled = direType == DireType.Fixed;
            }
        }

        /// <summary>
        /// 获得目标点
        /// </summary>
        /// <returns></returns>
        Vector3 GetTargetPos()
        {
            Vector3 targetPos = target.transform.position + new Vector3(0, target.Data.UnitConfig.Height / 2, 0);

            if (direType == DireType.Fixed)
            {
                // 精度散布
                targetPos += new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            }
            return targetPos;
        }

        /// <summary>
        /// 设置回收回调
        /// </summary>
        /// <param name="action">回收回调函数</param>
        public void SetCollectAction(Action<GameObject> action)
        {
            collectTraceAction = action;
        }

        public void SetHitAction(Action<BaseBattleUnit> action)
        {
            hitAction = action;
        }

        public void SetHitEffectPool(ObjPool _hitEffectPool)
        {
            hitEffectPool = _hitEffectPool;
        }

        /// <summary>
        /// 回收投射物
        /// </summary>
        private void Collect()
        {
            isInited = false;

            if (collectTraceAction != null)
            {
                collectTraceAction(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 击中
        /// </summary>
        private void Hit()
        {
            ///
            hitAction?.Invoke(target);
            if (hitEffectPool == null)
            {
                Collect();
            }
            else
            {
                GameObject hitEffect = hitEffectPool.Get();
                gameObject.SetActive(false);
                hitEffect.transform.position = transform.position;
                TimerMgr.Ins.AddTimer(this, delay: 2f, onComplete: () =>
                {
                    hitEffectPool.CollectOne(hitEffect);
                    Collect();
                });
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

            if (direType == DireType.Fixed)
            {
                // 固定抛物线轨迹
                UpdateFixedDire();
            }
            else if (true)
            {
                //跟随抛物线轨迹
                UpdateFllowDire();
            }
        }

        /// <summary>
        /// 固定抛物线轨迹
        /// </summary>
        private void UpdateFixedDire()
        {
            // 增加飞行进度
            flightProgress += Time.deltaTime * speed / Vector3.Distance(startPos, targetPos);
            flightProgress = Mathf.Clamp01(flightProgress); // 确保值在0-1之间

            // 计算当前位置（线性插值 + 抛物线高度）
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, flightProgress);

            // 添加抛物线高度 (使用sin曲线模拟抛物线，在中间点达到最大高度)
            float heightOffset = Mathf.Sin(flightProgress * Mathf.PI) * arcHeight;
            currentPos.y += heightOffset;

            // 计算移动方向和速度
            Vector3 direction = (currentPos - transform.position).normalized;
            Vector3 velocity = direction * speed;

            // 使用刚体移动，而不是直接修改transform位置
            rigid.velocity = velocity;

            // 更新朝向（始终朝向飞行方向）
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        /// <summary>
        /// 跟随抛物线轨迹
        /// </summary>
        void UpdateFllowDire()
        {
            // 目标丢失 直接触发击中
            if (target == null)
            {
                Hit();
                return;
            }

            // 获取目标当前位置
            if (target.isChangePosThisFrame)
                targetPos = GetTargetPos();

            // 计算方向向量
            Vector3 direction = (targetPos - transform.position).normalized;

            // 计算本帧移动距离
            float moveDistance = speed * Time.deltaTime;

            // 直接更新位置
            transform.position += direction * moveDistance;

            // 更新朝向（始终朝向目标）
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // 检查是否已经足够接近目标，如果是则触发击中效果
            float distanceToTarget = Vector3.Distance(transform.position, targetPos);
            if (distanceToTarget < 0.5f) // 可以根据需要调整这个阈值
            {
                Hit();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isInited) return;
            // 检查是否碰到了目标单位
            BaseBattleUnit hitUnit = other.gameObject.GetComponent<BaseBattleUnit>();
            if (hitUnit != null)
            {
                //自己
                if (hitUnit == owner)
                {
                    return;
                }
                // 敌人检测
                if (owner.IsEnemy(hitUnit))
                {
                    hitAction?.Invoke(hitUnit);
                }
            }
            Collect();
        }
    }

    /// <summary>
    /// 跟随类型
    /// </summary>
    public enum DireType
    {
        Fixed,  //固定轨迹
        Follow  //跟随
    }
}
