using System;
using Newtonsoft.Json;
using Rain.Core;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 战斗单位逻辑组件
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class BattleUnit : MonoBehaviour
    {
        [Header("组件")]
        public Animator animator;
        public Collider unitCollider;
        public GameObject shootPos;

        //事件
        public event Action<BattleUnit> OnDeath;

        //数据
        [SerializeField] private UnitData _data;        // 角色数据

        // 属性访问器
        public UnitData Data => _data;
        public bool IsDead => Data.isDead;
        public string UnitName => Data.Name;
        public float AttackTimer { get => Data.attackTimer; set => Data.attackTimer = value; }
        public Vector3? MovePos { get => Data.MovePos; set => Data.MovePos = value; }
        public BattleUnit AttackTarget { get => Data.AttackTarget; set => Data.AttackTarget = value; }
        public bool HasMoveTarget { get => Data.MovePos != null || Data.AttackTarget != null; }

        public UnitMoveController moveController;
        public UnitStateMachine stateMachine;
        public UnitStateType unitStateType => stateMachine.currentState.stateType;
        public NavMeshAgent agent => moveController.agent;

        /// <summary>
        /// 上次的位置
        /// </summary>
        Vector3 lastPos;


        private void Awake()
        {
            // 获取组件引用
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.Log($"BattleUnit找不到[动画]组件");
            }
            unitCollider = GetComponent<Collider>() ?? gameObject.AddComponent<CapsuleCollider>();
            if (unitCollider == null)
            {
                Debug.Log($"BattleUnit找不到[碰撞体]组件");
            }
            moveController = GetComponent<UnitMoveController>() ?? gameObject.AddComponent<UnitMoveController>();
            if (moveController == null)
            {
                Debug.Log($"BattleUnit找不到[UnitMoveController]组件");
            }
            stateMachine = new UnitStateMachine(this);
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (lastPos != transform.position)
            {
                lastPos = transform.position;
                MsgMgr.Ins.DispatchEvent(MsgEvent.RTSBattleUnitMove, this);
            }
            if (!Data.isDead)
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
            Data.Init(unitConfig);
            moveController.Init(Data);

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
                float distance = Vector3.Distance(transform.position, target.transform.position);
                Debug.Log($"命令{Data.Name}攻击{target.Data.Name},距离{distance}");
                Data.AttackTarget = target;
            }
        }

        /// <summary>
        /// 清除攻击目标
        /// </summary>
        public void ClearAttackTarget()
        {
            Data.AttackTarget = null;
        }

        /// <summary>
        /// 检查目标是否在攻击范围内
        /// </summary>
        /// <returns></returns>
        public bool IsTargetInAttackRange()
        {
            if (Data.AttackTarget == null) return false;

            float distance = Vector3.Distance(transform.position, Data.AttackTarget.transform.position);
            return distance <= Data.attackRange;
        }

        /// <summary>
        /// 执行攻击
        /// </summary>
        public void Attack()
        {
            if (Data.AttackTarget != null && !Data.AttackTarget.Data.isDead && IsEnemy(Data.AttackTarget))
            {
                AttackTimer = Time.time;
                Data.AttackTarget.Hurt(Data.attack);
                Debug.Log($"{Data.Name} 攻击了 {Data.AttackTarget.Data.Name}，造成 {Data.attack} 点伤害");
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage"></param>
        public void Hurt(int damage)
        {
            if (Data.isDead)
                return;

            float lastHp = Data.hp;
            Data.hp -= damage;
            if (Data.hp <= 0)
            {
                Data.hp = 0;
                stateMachine.ChangeState(new DieState());
            }
            MsgMgr.Ins.DispatchEvent(MsgEvent.RTSUnitHPChange, this, lastHp);
        }

        /// <summary>
        /// 死亡
        /// </summary>
        public void Dead()
        {
            Data.isDead = true;
            unitCollider.enabled = false;
            agent.isStopped = true;
            agent.enabled = false;
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
            return !Data.isDead;
        }

        /// <summary>
        /// 面向目标
        /// </summary>
        public void LookAtTarget()
        {
            if (Data.AttackTarget == null) return;

            Vector3 targetPosition = new Vector3(
                Data.AttackTarget.transform.position.x,
                transform.position.y,
                Data.AttackTarget.transform.position.z
            );
            transform.LookAt(targetPosition);
        }

        /// <summary>
        /// 单位移动
        /// </summary>
        public void UnitMove(Vector3 targetPos)
        {

        }

        /// <summary>
        /// 检查是否可以攻击
        /// </summary>
        public bool CanAttack()
        {
            if (Data.AttackTarget == null)
                return false;
            if (Time.time - AttackTimer <= Data.attackInterval) return false;

            // 检查距离是否在攻击范围内
            bool isTargetInAttackRange = IsTargetInAttackRange();
            return isTargetInAttackRange;
        }
    }
}