using System.Xml.Linq;
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
        private UnitData data; // 引用角色数据


        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>() ?? gameObject.AddComponent<NavMeshAgent>();
        }

        // 初始化：关联角色数据并设置初始速度
        public void Init(UnitData _data)
        {
            this.data = _data;

            agent.speed = this.data.moveSpeed;
            agent.updateRotation = false;                       // 禁用自动旋转，由脚本控制
            agent.stoppingDistance = this.data.attackRange - 0.1f;  // 停止距离,稍微小于攻击范围
            agent.acceleration = 999;
        }

        // 移动到目标点
        public void MoveTo(Vector3 targetPosition)
        {
            ClearMoveTarget();
            data.MovePos = targetPosition;
            agent.SetDestination(targetPosition);
            agent.isStopped = false;
        }

        public void MoveTo(BattleUnit battleUnit)
        {
            ClearMoveTarget();
            data.AttackTarget = battleUnit;
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
        public void SetMoveTarget(BattleUnit _battleUnit)
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
