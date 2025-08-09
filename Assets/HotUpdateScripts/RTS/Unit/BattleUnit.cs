using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

namespace Rain.Core.RTS
{
    /// <summary>
    /// 战斗单位逻辑组件
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class BattleUnit : MonoBehaviour
    {
        [Header("引用")]
        public Transform attackTarget; // 攻击目标

        // 组件引用
        public Animator animator;
        public CapsuleCollider capsuleCollider;

        //事件
        public event Action<BattleUnit> OnDeath;
        public bool HasMoveTarget => moveController.HasMoveTarget;
        public Vector3 MoveTarget => moveController.MoveTarget;

        //数据
        [SerializeField] private UnitData _data;        // 角色数据

        // 属性访问器
        public UnitData Data => _data;
        public bool IsDead => _data.isDead;
        public string UnitName => Data.Name;
        public float AttackTimer => Data.attackTimer;

        public UnitMoveController moveController;
        public UnitStateMachine stateMachine;
        public UnitStateType unitStateType => stateMachine.currentState.stateType;
        public NavMeshAgent agent => moveController.agent;


        private void Awake()
        {
            // 获取组件引用
            animator = GetComponent<Animator>();
            capsuleCollider = GetComponent<CapsuleCollider>() ?? gameObject.AddComponent<CapsuleCollider>();
            moveController = GetComponent<UnitMoveController>() ?? gameObject.AddComponent<UnitMoveController>();
            stateMachine = new UnitStateMachine(this);
        }

        private void Start()
        {
        }



        private void Update()
        {
            if (!_data.isDead)
            {
                stateMachine.Update();
            }
        }

        private void OnDestroy()
        {
            // 从战斗管理器注销
            BattleMgr.Ins.UnregisterUnit(this);
        }

        public void InitData(UnitConfig unitConfig)
        {
            _data.Init(unitConfig);
            moveController.Init(_data);

            // 注册到战斗管理器
            BattleMgr.Ins.RegisterUnit(this);

            // 初始状态为空闲
            stateMachine.ChangeState(new IdleState());
        }

        /// <summary>
        /// 设置移动目标
        /// </summary>
        /// <param name="target"></param>
        public void SetMoveTarget(Vector3 target)
        {
            moveController.SetMoveTarget(target);
            stateMachine.UpdateState();
        }

        /// <summary>
        /// 清除移动目标
        /// </summary>
        public void ClearMoveTarget()
        {
            moveController.ClearMoveTarget();
        }

        /// <summary>
        /// 设置攻击目标
        /// </summary>
        /// <param name="target"></param>
        public void SetAttackTarget(BattleUnit target)
        {
            if (IsEnemy(target))
            {
                _data.targetEnemy = target;
            }
        }

        /// <summary>
        /// 清除攻击目标
        /// </summary>
        public void ClearAttackTarget()
        {
            _data.targetEnemy = null;
        }

        /// <summary>
        /// 检查目标是否在攻击范围内
        /// </summary>
        /// <returns></returns>
        public bool IsTargetInAttackRange()
        {
            if (_data.targetEnemy == null) return false;

            float distance = Vector3.Distance(transform.position, _data.targetEnemy.transform.position);
            return distance <= _data.attackRange;
        }

        /// <summary>
        /// 执行攻击
        /// </summary>
        public void AttackTarget()
        {
            if (_data.targetEnemy != null && !_data.targetEnemy.Data.isDead && IsEnemy(_data.targetEnemy))
            {
                _data.targetEnemy.TakeDamage(_data.attack);
                Debug.Log($"{_data.Name} 攻击了 {_data.targetEnemy.Data.Name}，造成 {_data.Name} 点伤害");
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            if (_data.isDead) return;

            _data.currentHealth -= damage;
            if (_data.currentHealth <= 0)
            {
                _data.currentHealth = 0;
                stateMachine.ChangeState(new DieState());
            }
        }

        /// <summary>
        /// 面向目标
        /// </summary>
        /// <param name="target"></param>
        public void FaceTarget(Transform target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // 保持在同一水平面上

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        /// <summary>
        /// 检查是否为有效目标
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsValidTarget(BattleUnit target)
        {
            return target != null && !target.Data.isDead && IsEnemy(target);
        }

        /// <summary>
        /// 检查两个单位是否为敌对关系
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEnemy(BattleUnit other)
        {
            bool res = BattleMgr.Ins.IsEnemy(this, other);
            return res;
        }

        /// <summary>
        /// 获取阵营关系
        /// </summary>
        /// <param name="ownFaction"></param>
        /// <param name="_otherFaction"></param>
        /// <returns></returns>
        public Relation GetFactionRelation(Faction _otherFaction)
        {
            Relation relation = BattleMgr.Ins.GetFactionRelation(Data.faction, _otherFaction);
            return relation;
        }

        // 检查单位是否存活
        public bool IsAlive()
        {
            return !_data.isDead;
        }

        /// <summary>
        /// 面向目标
        /// </summary>
        public void LookAtTarget()
        {
            if (_data.targetEnemy == null) return;

            Vector3 targetPosition = new Vector3(
                _data.targetEnemy.transform.position.x,
                transform.position.y,
                _data.targetEnemy.transform.position.z
            );
            transform.LookAt(targetPosition);
        }

        ////////////////////////////////////////////////////

        ///// <summary>
        ///// 设置移动目标点
        ///// </summary>
        //public void SetDestination(Vector3 targetPosition)
        //{
        //    if (_data.isDead || StateMachine.CurrentState == BattleUnitState.Hurt) return;

        //    agent.SetDestination(targetPosition);

        //    // 根据距离自动选择走或跑
        //    float distance = Vector3.Distance(transform.position, targetPosition);
        //    currentState = distance > 5f ? BattleUnitState.Run : BattleUnitState.Walk;
        //    agent.speed = currentState == BattleUnitState.Run ? runSpeed : walkSpeed;
        //}

        ///// <summary>
        ///// 触发攻击
        ///// </summary>
        //public void TriggerAttack(Transform target)
        //{
        //    if (isDead || currentState == BattleUnitState.Hurt || attackTimer > 0) return;

        //    attackTarget = target;
        //    currentState = BattleUnitState.Attack;
        //    isAttacking = true;
        //    animator.SetBool(isAttackingHash, true);
        //    agent.ResetPath(); // 停止移动
        //}

        //public void SetCamp(bool isPlayer)
        //{
        //    IsPlayerCamp = isPlayer;
        //}

        //public void PrepareForBattle()
        //{
        //    IsDead = false;
        //    // 准备战斗的逻辑
        //}

        ///// <summary>
        ///// 死亡
        ///// </summary>
        //public void Die()
        //{
        //    if (isDead) return;

        //    isDead = true;
        //    currentState = BattleUnitState.Death;
        //    animator.SetBool(isDeadHash, true);
        //    agent.enabled = false; // 死亡后停止移动
        //    capsuleCollider.enabled = false; // 死亡后禁用碰撞体
        //}

        //public void Pause()
        //{
        //    // 暂停单位行为
        //}

        //public void Resume()
        //{
        //    // 恢复单位行为
        //}

        //public void OnBattleEnd(bool isVictory)
        //{
        //    // 战斗结束时的处理
        //}

        /// <summary>
        /// 检查是否可以攻击
        /// </summary>
        public bool CanAttack()
        {
            if (attackTarget == null) return false;
            if (Time.time - AttackTimer <= Data.attackInterval) return false;

            // 检查距离是否在攻击范围内
            float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);
            if (distanceToTarget > _data.attackRange) return false;

            return true;
        }

        ///// <summary>
        ///// 朝向目标方向旋转
        ///// </summary>
        //private void RotateTowards(Vector3 direction)
        //{
        //    if (direction.sqrMagnitude < 0.1f) return;

        //    Quaternion targetRotation = Quaternion.LookRotation(direction);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}

        ///// <summary>
        ///// 攻击动画事件（需在动画关键帧中设置）
        ///// </summary>
        //public void OnAttackHit()
        //{
        //    if (attackTarget != null && CanAttack())
        //    {
        //        // 这里可以添加对目标造成伤害的逻辑
        //        Debug.Log($"对 {attackTarget.name} 造成 {attackDamage} 点伤害");
        //    }
        //}

        /// <summary>
        /// 播放受伤动画
        /// </summary>
        public void PlayHurtAnimation()
        {
        }
    }
}