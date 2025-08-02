using UnityEngine;
using UnityEngine.AI;

namespace Rain.Core.RTS
{
    /// <summary>
    /// 战斗角色控制器
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(CapsuleCollider))]
    public class UnitMoveController : MonoBehaviour
    {
        public NavMeshAgent agent;
        private UnitData _data; // 引用角色数据

        public bool HasMoveTarget { get; private set; }
        public Vector3 MoveTarget { get; private set; }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>() ?? gameObject.AddComponent<NavMeshAgent>();
        }

        // 初始化：关联角色数据并设置初始速度
        public void Init(UnitData data)
        {
            _data = data;

            agent.speed = _data.moveSpeed;
            agent.updateRotation = false;                       // 禁用自动旋转，由脚本控制
            agent.stoppingDistance = _data.attackRange - 0.1f;  // 停止距离,稍微小于攻击范围
            agent.acceleration = 999;
        }

        // 移动到目标点
        public void MoveTo(Vector3 targetPosition)
        {
            if (_data.isDead) return; // 死亡状态不移动
            agent.SetDestination(targetPosition);
            agent.isStopped = false;
        }

        // 停止移动
        public void Stop()
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

        public bool IsAtTarget()
        {
            if (!HasMoveTarget) return true;

            float distanceToTarget = Vector3.Distance(transform.position, MoveTarget);
            return distanceToTarget <= agent.stoppingDistance + 0.1f;
        }

        public void ClearMoveTarget()
        {
            HasMoveTarget = false;
            MoveTarget = Vector3.zero;
        }

        // 追击目标单位
        public void ChaseTarget(Transform target)
        {
            if (_data.isDead || target == null) return;
            MoveTo(target.position);
        }

        // 获取当前移动速度
        public float GetCurrentSpeed()
        {
            return agent.velocity.magnitude;
        }
    }
}
