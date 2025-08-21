using System;
using System.Xml.Linq;
using UnityEngine;

namespace Rain.RTS.Core
{
	/// <summary>
	/// 单位属性数据类
	/// </summary>
	[Serializable]
	public class UnitData
	{
		[Header("数据源")]
		[SerializeField]
		private UnitConfig unitConfig;
		public UnitConfig UnitConfig => unitConfig;
		[Header("基础属性")]
		public string unitId;           // 唯一ID
		public bool isPlayerUnit;       // 是否为玩家单位
		public float moveSpeed;         // 移动速度
		public float runSpeed;          // 移动速度
		public float rotationSpeed;     // 转向速度
		public int attack;              // 攻击力
		public float attackRange;       // 攻击范围
		public float attackInterval;    // 攻击间隔
		public int maxHealth;			// 最大生命值
		public Faction faction;         // 阵营
		public UnitType unitType;       // 单位类型

		[Header("运行时状态")]
		public int hp;                  // 生命值
		public bool isDead = false;     // 是否死亡
		private Vector3? movePos;       // 目标位置
		public Vector3? MovePos { get => movePos; set => movePos = value; }
		private BattleUnit attackTarget;  // 目标敌人
		public BattleUnit AttackTarget { get => attackTarget; set => attackTarget = value; }
		public float attackTimer;        // 上次攻击时间
		public const float walkRange = 3;// 切换走路的距离

		[Header("运行时状态")]
		/// <summary>
		/// 跑步动画
		/// </summary>
		public const float runAnimationSpeed = 0.3f;

		//名字
		public string Name => LangMgr.Ins.Get(unitConfig.name);


		public void Init(UnitConfig config)
		{
			unitId = Guid.NewGuid().ToString("N");
			unitConfig = config;
			maxHealth = unitConfig.hp;
			hp = maxHealth;
			UpdateAttributes();
		}

		private void UpdateAttributes()
		{
			unitType = unitConfig.unitType;
			faction = unitConfig.faction;
			hp = unitConfig.hp;
			moveSpeed = unitConfig.moveSpeed;
			runSpeed = moveSpeed * 1.5f;
			rotationSpeed = 10f;
			attack = unitConfig.attack;
			attackRange = unitConfig.attackRange;
			attackInterval = unitConfig.attackInterval;
		}
	}

	// 状态枚举（与动画状态对应）
	public enum BattleUnitState
	{
		Idle,
		Walk,
		Run,
		Attack,
		Hurt,
		Death
	}
}