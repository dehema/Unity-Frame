using UnityEngine;
using UnityEngine.AI;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 战斗角色控制器
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(CapsuleCollider))]
    public class UnitMoveController : MonoBehaviour
    {
        public NavMeshAgent agent;
        private UnitData data; // 引用角色数据

        public void Init()
        {
            agent = GetComponent<NavMeshAgent>() ?? gameObject.AddComponent<NavMeshAgent>();
            agent.speed = this.data.moveSpeed;
            agent.updateRotation = false;                       // 禁用自动旋转，由脚本控制
            agent.stoppingDistance = 0.1f;
            agent.acceleration = 999;
            agent.height = data.UnitConfig.Height;
        }

        // 初始化：关联角色数据并设置初始速度
        public void InitData(UnitData _data)
        {
            this.data = _data;
        }

        // 移动到目标点
        public void MoveTo(Vector3 targetPosition)
        {
            ClearMoveTarget();
            data.MovePos = targetPosition;
            agent.stoppingDistance = 0.1f;
            agent.SetDestination(targetPosition);
            agent.isStopped = false;
        }

        public void MoveToAttack(BaseBattleUnit battleUnit)
        {
            ClearMoveTarget();
            data.AttackTarget = battleUnit;
            agent.stoppingDistance = data.attackRange;
            agent.SetDestination(battleUnit.transform.position);
            agent.isStopped = false;
        }

        // 停止移动
        public void Stop()
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

        /// <summary>
        /// 设置移动目标
        /// </summary>
        /// <param name="target"></param>
        public void SetMoveTarget(Vector3 _movePos)
        {
            ClearMoveTarget();
            data.MovePos = _movePos;
        }

        /// <summary>
        /// 设置移动目标
        /// </summary>
        /// <param name="target"></param>
        public void SetMoveTarget(BaseBattleUnit _battleUnit)
        {
            ClearMoveTarget();
            data.AttackTarget = _battleUnit;
        }

        public void ClearMoveTarget()
        {
            data.MovePos = null;
            data.AttackTarget = null;
        }

        // 追击目标单位
        public void ChaseTarget(Transform target)
        {
            if (data.isDead || target == null) return;
            MoveTo(target.position);
        }

        // 获取当前移动速度
        public float GetCurrentSpeed()
        {
            return agent.velocity.magnitude;
        }
    }
}
