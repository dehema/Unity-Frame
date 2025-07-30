using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(CapsuleCollider))]
public class UnitController_RTS : MonoBehaviour
{
    // 状态枚举（与动画状态对应）
    public enum SoldierState
    {
        Idle,
        Walk,
        Run,
        Attack,
        Hurt,
        Death
    }

    // 公共参数
    [Header("移动设置")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("攻击设置")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("引用")]
    public Transform attackTarget; // 攻击目标

    // 组件引用
    private Animator animator;
    private NavMeshAgent agent;
    private CapsuleCollider capsuleCollider;

    // 状态控制变量
    private SoldierState currentState;
    private float currentSpeed;
    private float attackTimer;
    private bool isAttacking;
    private bool isDead;

    // 动画参数哈希（优化性能）
    private int speedHash = Animator.StringToHash("Speed");
    private int isAttackingHash = Animator.StringToHash("IsAttacking");
    private int isHurtHash = Animator.StringToHash("IsHurt");
    private int isDeadHash = Animator.StringToHash("IsDead");

    private void Awake()
    {
        // 获取组件引用
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // 初始化导航代理设置
        agent.updateRotation = false; // 禁用自动旋转，由脚本控制
        agent.speed = walkSpeed;    //默认速度
        agent.stoppingDistance = 1; //到达目标的距离
    }

    private void Start()
    {
        // 初始状态为站立
        currentState = SoldierState.Idle;
    }

    private void Update()
    {
        Debug.Log(agent.velocity);
        // 死亡状态不执行任何更新
        if (isDead) return;

        // 更新攻击冷却
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // 根据当前状态执行对应逻辑
        switch (currentState)
        {
            case SoldierState.Idle:
                UpdateIdleState();
                break;
            case SoldierState.Walk:
            case SoldierState.Run:
                UpdateMovementState();
                break;
            case SoldierState.Attack:
                UpdateAttackState();
                break;
            case SoldierState.Hurt:
                UpdateHurtState();
                break;
        }
    }

    #region 状态更新方法
    private void UpdateIdleState()
    {
        // 检查是否需要移动
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            // 根据目标距离判断走或跑
            float distanceToTarget = Vector3.Distance(transform.position, agent.destination);
            currentState = distanceToTarget > 5f ? SoldierState.Run : SoldierState.Walk;
            agent.speed = currentState == SoldierState.Run ? runSpeed : walkSpeed;
        }
        // 检查是否可以攻击
        else if (CanAttack() && attackTimer <= 0)
        {
            currentState = SoldierState.Attack;
            isAttacking = true;
            animator.SetBool(isAttackingHash, true);
            agent.ResetPath(); // 停止移动
        }
    }

    /// <summary>
    /// 根据距离计算归一化速度
    /// </summary>
    /// <param name="distance">距离值</param>
    /// <returns>归一化后的速度值（0.3-1.0之间）</returns>
    public static float GetAnimationSpeed(float distance)
    {
        // 确保距离不为负数
        distance = Mathf.Max(0, distance);

        if (distance > 10f)
        {
            return 1.0f;
        }
        else if (distance <= 2f)
        {
            return 0.3f;
        }
        else
        {
            // 线性插值计算2到10之间的速度值
            // 映射公式：速度 = 0.3 + (距离 - 2) * (0.7 / 8)
            return 0.3f + (distance - 2f) * 0.7f / 8f;
        }
    }

    private void UpdateMovementState()
    {
        // 到达目的地切换到站立
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = SoldierState.Idle;
            animator.SetFloat(speedHash, 0);
            return;
        }

        // 更新动画速度参数
        float remainDistance = agent.remainingDistance - agent.stoppingDistance;
        float animationSpeed = GetAnimationSpeed(remainDistance);
        animator.SetFloat(speedHash, animationSpeed);

        //根据自身速度设置动画是速度
        //currentSpeed = agent.velocity.magnitude;
        //float normalizedSpeed = currentSpeed / runSpeed; // 归一化到0~1
        //Debug.Log("速度: " + normalizedSpeed);
        //animator.SetFloat(speedHash, normalizedSpeed);

        // 面向移动方向
        RotateTowards(agent.velocity.normalized);

        // 检查是否可以攻击
        if (CanAttack() && attackTimer <= 0)
        {
            currentState = SoldierState.Attack;
            isAttacking = true;
            animator.SetBool(isAttackingHash, true);
            agent.ResetPath(); // 停止移动
        }
        // 切换走/跑状态
        else if (currentState == SoldierState.Walk && currentSpeed >= runSpeed * 0.8f)
        {
            currentState = SoldierState.Run;
            agent.speed = runSpeed;
        }
        else if (currentState == SoldierState.Run && currentSpeed <= walkSpeed * 1.2f)
        {
            currentState = SoldierState.Walk;
            agent.speed = walkSpeed;
        }
    }

    private void UpdateAttackState()
    {
        // 如果不在攻击状态，返回移动状态
        if (!isAttacking)
        {
            // 检查是否有移动目标
            if (agent.hasPath)
            {
                currentState = Vector3.Distance(transform.position, agent.destination) > 5f
                    ? SoldierState.Run
                    : SoldierState.Walk;
                agent.speed = currentState == SoldierState.Run ? runSpeed : walkSpeed;
            }
            else
            {
                currentState = SoldierState.Idle;
            }
            return;
        }

        // 攻击时面向目标
        if (attackTarget != null)
        {
            RotateTowards(attackTarget.position - transform.position);
        }
    }

    private void UpdateHurtState()
    {
        // 受伤动画播放完毕后回到之前的状态
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1f && !stateInfo.loop)
        {
            // 清除受伤状态
            animator.ResetTrigger(isHurtHash);

            // 恢复移动能力
            agent.enabled = true;

            // 回到之前的状态（除了死亡）
            if (agent.hasPath)
            {
                currentState = Vector3.Distance(transform.position, agent.destination) > 5f
                    ? SoldierState.Run
                    : SoldierState.Walk;
            }
            else
            {
                currentState = SoldierState.Idle;
            }
        }
    }
    #endregion

    #region 公共方法（外部调用）
    /// <summary>
    /// 设置移动目标点
    /// </summary>
    public void SetDestination(Vector3 targetPosition)
    {
        if (isDead || currentState == SoldierState.Hurt) return;

        agent.SetDestination(targetPosition);

        // 根据距离自动选择走或跑
        float distance = Vector3.Distance(transform.position, targetPosition);
        currentState = distance > 5f ? SoldierState.Run : SoldierState.Walk;
        agent.speed = currentState == SoldierState.Run ? runSpeed : walkSpeed;
    }

    /// <summary>
    /// 触发攻击
    /// </summary>
    public void TriggerAttack(Transform target)
    {
        if (isDead || currentState == SoldierState.Hurt || attackTimer > 0) return;

        attackTarget = target;
        currentState = SoldierState.Attack;
        isAttacking = true;
        animator.SetBool(isAttackingHash, true);
        agent.ResetPath(); // 停止移动
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // 这里可以添加伤害处理逻辑，例如生命值减少等

        currentState = SoldierState.Hurt;
        animator.SetTrigger(isHurtHash);
        agent.enabled = false; // 受伤时停止移动
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        currentState = SoldierState.Death;
        animator.SetBool(isDeadHash, true);
        agent.enabled = false; // 死亡后停止移动
        capsuleCollider.enabled = false; // 死亡后禁用碰撞体
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 检查是否可以攻击
    /// </summary>
    private bool CanAttack()
    {
        if (attackTarget == null) return false;

        // 检查距离是否在攻击范围内
        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);
        if (distanceToTarget > attackRange) return false;

        // 检查是否可以看到目标（简单的视线检测）
        if (!Physics.Linecast(transform.position + Vector3.up, attackTarget.position + Vector3.up, out RaycastHit hit))
        {
            return true;
        }

        return hit.collider.transform == attackTarget;
    }

    /// <summary>
    /// 朝向目标方向旋转
    /// </summary>
    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 攻击动画事件（需在动画关键帧中设置）
    /// </summary>
    public void OnAttackHit()
    {
        if (attackTarget != null && CanAttack())
        {
            // 这里可以添加对目标造成伤害的逻辑
            Debug.Log($"对 {attackTarget.name} 造成 {attackDamage} 点伤害");
        }
    }

    /// <summary>
    /// 攻击动画结束事件（需在动画关键帧中设置）
    /// </summary>
    public void OnAttackEnd()
    {
        isAttacking = false;
        animator.SetBool(isAttackingHash, false);
        attackTimer = attackCooldown; // 开始冷却
    }
    #endregion

    // 绘制Gizmos辅助调试
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        // 绘制攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 绘制移动目标
        if (agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(agent.destination, 0.5f);
        }
    }
}
