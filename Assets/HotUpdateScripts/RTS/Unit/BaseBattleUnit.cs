using System;
using Rain.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 战斗单位基类，所有具体兵种类型都应该继承此类
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class BaseBattleUnit : MonoBehaviour
    {
        [Header("组件")]
        public Animator animator;
        public Collider unitCollider;
        public UnitBodyPart bodyPart;

        //事件
        public event Action<BaseBattleUnit> OnDeath;

        //数据
        [SerializeField] protected UnitData _data;        // 角色数据

        // 属性访问器
        public UnitData Data => _data;
        public bool IsDead => Data.isDead;
        public string UnitName => Data.Name;
        public float AttackTimer { get => Data.attackTimer; set => Data.attackTimer = value; }
        public Vector3? MovePos { get => Data.movePos; set => Data.movePos = value; }
        public BaseBattleUnit AttackTarget { get => Data.attackTarget; set => Data.attackTarget = value; }
        public bool HasMoveTarget { get => Data.movePos != null || Data.attackTarget != null; }
        public UnitControlMode ControlMode { get => Data.controlMode; set => Data.controlMode = value; }

        public UnitMoveController moveController;
        public UnitStateMachine stateMachine;
        public UnitStateType unitStateType => stateMachine.currentState.stateType;
        public NavMeshAgent agent => moveController.navAgent;

        /// <summary>
        /// 攻击策略
        /// </summary>
        public IAttackStrategy attackStrategy;

        /// <summary>
        /// 上次的位置
        /// </summary>
        protected Vector3 lastPos;
        /// <summary>
        /// 当前帧是否改变位置
        /// </summary>
        public bool IsMoveThisFrame = false;

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            //更新自身位置
            IsMoveThisFrame = lastPos != transform.position;
            if (IsMoveThisFrame)
            {
                lastPos = transform.position;
                MsgMgr.Ins.DispatchEvent(MsgEvent.RTSBattleUnitMove, this);
            }
            //更新状态机
            if (!Data.isDead)
            {
                stateMachine.Update();
            }
        }

        protected virtual void OnDestroy()
        {
            // 从战斗管理器注销
            BattleMgr.Ins.UnregisterUnit(this);
        }

        /// <summary>
        /// 初始化攻击策略，由子类实现
        /// </summary>
        protected abstract void InitAttackStrategy();

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            // 获取组件引用
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"BattleUnit找不到[动画]组件");
            }
            unitCollider = GetComponent<Collider>() ?? gameObject.AddComponent<CapsuleCollider>();
            if (unitCollider == null)
            {
                Debug.LogError($"BattleUnit找不到[碰撞体]组件");
            }
            if (unitCollider is CapsuleCollider)
            {
                CapsuleCollider capsuleCollider = unitCollider as CapsuleCollider;
                capsuleCollider.height = Data.unitConfig.Height;
                capsuleCollider.center = new Vector3(0, Data.unitConfig.Height / 2, 0);
            }
            moveController = GetComponent<UnitMoveController>() ?? gameObject.AddComponent<UnitMoveController>();
            if (moveController == null)
            {
                Debug.LogError($"BattleUnit找不到[UnitMoveController]组件");
            }
            moveController.InitData(Data);
            moveController.Init();

            bodyPart = GetComponent<UnitBodyPart>();

            // 状态机
            stateMachine = new UnitStateMachine(this);

            // 初始化攻击策略
            InitAttackStrategy();

            // 注册到战斗管理器
            BattleMgr.Ins.RegisterUnit(this);

            // 初始状态为空闲
            stateMachine.ChangeState(new IdleState());
        }

        public virtual void InitData(UnitConfig unitConfig, UnitInitData _initData = null)
        {
            _data = new UnitData();
            Data.Init(unitConfig);
            gameObject.name = $"{Data.unitType}_{Data.unitId}";
            if (_initData != null)
            {
                Data.faction = _initData.faction;
            }
        }

        /// <summary>
        /// 设置移动目标
        /// </summary>
        /// <param name="target"></param>
        public virtual void SetMoveTarget(Vector3 target)
        {
            moveController.SetMoveTarget(target);
            stateMachine.UpdateState();
        }

        /// <summary>
        /// 清除移动目标
        /// </summary>
        public virtual void ClearMoveTarget()
        {
            moveController.ClearMoveTarget();
        }

        /// <summary>
        /// 设置攻击目标
        /// </summary>
        /// <param name="target"></param>
        public virtual void SetAttackTarget(BaseBattleUnit target)
        {
            if (IsEnemy(target))
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                //Debug.Log($"命令{Data.Name}攻击{target.Data.Name},距离{distance}");
                Data.attackTarget = target;
            }
        }

        /// <summary>
        /// 清除攻击目标
        /// </summary>
        public virtual void ClearAttackTarget()
        {
            Data.attackTarget = null;
        }

        /// <summary>
        /// 检查目标是否在攻击范围内
        /// </summary>
        /// <returns></returns>
        public virtual bool IsTargetInAttackRange()
        {
            if (Data.attackTarget == null) return false;

            float distance = Vector3.Distance(transform.position, Data.attackTarget.transform.position);
            return distance <= Data.FloatAttackTargetDistance;
        }

        /// <summary>
        /// 执行攻击
        /// </summary>
        public virtual void PerformAttackOrder()
        {
            //Debug.Log($"[{Data.Name}] 攻击了 [{AttackTarget.Data.Name}]");
            AttackTimer = Time.time;
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage"></param>
        public virtual void Hurt(float damage)
        {
            if (Data.isDead)
                return;

            float lastHp = Data.hp;
            //Debug.Log($"[{UnitName}] 受到 {damage} 点伤害");
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
        public virtual void Dead()
        {
            Data.isDead = true;
            unitCollider.enabled = false;
            agent.isStopped = true;
            agent.enabled = false;

            // 触发死亡事件
            OnDeath?.Invoke(this);

            BattleMgr.Ins.OnUnitDied(this);
        }

        /// <summary>
        /// 面向目标
        /// </summary>
        /// <param name="target"></param>
        public virtual void FaceTarget(Transform target)
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
        public virtual bool IsValidTarget(BaseBattleUnit target)
        {
            return target != null && !target.Data.isDead && IsEnemy(target);
        }

        /// <summary>
        /// 检查两个单位是否为敌对关系
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IsEnemy(BaseBattleUnit other)
        {
            bool res = BattleMgr.Ins.IsEnemy(this, other);
            return res;
        }

        /// <summary>
        /// 获取阵营关系
        /// </summary>
        /// <param name="_otherFaction"></param>
        /// <returns></returns>
        public virtual CampRelation GetFactionRelation(Faction _otherFaction)
        {
            CampRelation relation = BattleMgr.Ins.GetFactionRelation(Data.faction, _otherFaction);
            return relation;
        }

        // 检查单位是否存活
        public virtual bool IsAlive()
        {
            return !Data.isDead;
        }

        /// <summary>
        /// 面向目标
        /// </summary>
        public virtual void LookAtTarget()
        {
            if (Data.attackTarget == null) return;

            Vector3 targetPosition = new Vector3(Data.attackTarget.transform.position.x, transform.position.y, Data.attackTarget.transform.position.z);
            transform.LookAt(targetPosition);
        }

        /// <summary>
        /// 单位移动
        /// </summary>
        public virtual void UnitMove(Vector3 targetPos)
        {
        }

        /// <summary>
        /// 检查是否可以攻击
        /// </summary>
        public virtual bool CanAttack()
        {
            if (Data.attackTarget == null)
                return false;
            if (Time.time - AttackTimer <= Data.attackInterval) return false;

            // 检查距离是否在攻击范围内
            bool isTargetInAttackRange = IsTargetInAttackRange();
            return isTargetInAttackRange;
        }

        /// <summary>
        /// 伤害系数
        /// </summary>
        /// <param name="_damageFactor"></param>
        public void SetDamageFactor(float _damageFactor = 1)
        {
            Data.damageFactor = _damageFactor;
        }

        /// <summary>
        /// 移动（会切换状态机）
        /// </summary>
        /// <param name="_targetPos"></param>
        public void UnitMoveAndChangeState(Vector3 _targetPos)
        {
            if (!IsAlive())
            {
                return;
            }
            if (stateMachine.currentState.stateType != UnitStateType.Move)
            {
                SetMoveTarget(_targetPos);
                stateMachine.ChangeState(new MoveState());
            }
            else
            {
                MoveState moveState = stateMachine.currentState as MoveState;
                moveState.MoveTo(_targetPos);
            }
        }


        /// <summary>
        /// 自动寻找并攻击最近的敌人
        /// </summary>
        public void AutoFindAndAttackNearestEnemy()
        {
            BaseBattleUnit nearestEnemy = BattleMgr.Ins.FindNearestEnemy(this);
            if (nearestEnemy != null)
            {
                AttackUnitAndChangeState(nearestEnemy);
            }
        }

        /// <summary>
        /// 攻击（会切换状态机）
        /// </summary>
        /// <param name="_target"></param>
        public void AttackUnitAndChangeState(BaseBattleUnit _target)
        {
            if (!IsAlive())
            {
                return;
            }
            if (IsAlive() && IsEnemy(_target))
            {
                ClearMoveTarget();
                SetAttackTarget(_target);

                if (IsTargetInAttackRange())
                {
                    if (stateMachine.currentState.stateType != UnitStateType.Attack && CanAttack())
                    {
                        //直接攻击
                        stateMachine.ChangeState(new AttackState());
                    }
                    else
                    {
                        stateMachine.ChangeState(new IdleState());
                    }
                }
                else
                {
                    //移动&攻击
                    if (stateMachine.currentState.stateType == UnitStateType.Move)
                    {
                        MoveState moveState = stateMachine.currentState as MoveState;
                        moveState.MoveAndAttack(_target);
                    }
                    else
                    {
                        stateMachine.ChangeState(new MoveState());
                    }
                }
            }
        }
    }

}
