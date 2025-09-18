using dnlib.DotNet.Writer;
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
        public NavMeshAgent navAgent;
        private UnitData data; // 引用角色数据

        public void Init()
        {
            navAgent = GetComponent<NavMeshAgent>() ?? gameObject.AddComponent<NavMeshAgent>();
            navAgent.speed = this.data.moveSpeed;
            navAgent.updateRotation = false;                       // 禁用自动旋转，由脚本控制
            navAgent.stoppingDistance = 0.1f;
            navAgent.acceleration = 999;
            navAgent.height = data.unitConfig.Height;
            navAgent.radius = data.unitConfig.ModleRadius;
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
            data.movePos = targetPosition;
            navAgent.stoppingDistance = 0.1f;
            navAgent.SetDestination(targetPosition);
            navAgent.isStopped = false;
        }

        public void MoveToAttack(BaseBattleUnit _target)
        {
            ClearMoveTarget();
            data.attackTarget = _target;
            navAgent.stoppingDistance = data.FloatAttackTargetDistance;

            NavMeshAgent targetNavAgent = _target.moveController.navAgent;
            Vector3 dire = transform.position - _target.transform.position;
            if (_target.IsMoveThisFrame && targetNavAgent != null)
            {
                // 目标在移动，预测位置
                Vector3 predictedPos = _target.transform.position + dire * Time.deltaTime * targetNavAgent.speed;
                navAgent.SetDestination(predictedPos);
            }
            else
            {
                // 目标静止，直接移动到目标附近
                navAgent.SetDestination(_target.transform.position);
            }
            navAgent.isStopped = false;
        }

        // 停止移动
        public void Stop()
        {
            navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;
            navAgent.isStopped = true;
        }

        /// <summary>
        /// 设置移动目标
        /// </summary>
        /// <param name="target"></param>
        public void SetMoveTarget(Vector3 _movePos)
        {
            ClearMoveTarget();
            data.movePos = _movePos;
        }

        /// <summary>
        /// 设置移动目标
        /// </summary>
        /// <param name="target"></param>
        public void SetMoveTarget(BaseBattleUnit _battleUnit)
        {
            ClearMoveTarget();
            data.attackTarget = _battleUnit;
        }

        public void ClearMoveTarget()
        {
            data.movePos = null;
            data.attackTarget = null;
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
            return navAgent.velocity.magnitude;
        }
    }
}
