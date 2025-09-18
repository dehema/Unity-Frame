using System.Collections;
using System.Collections.Generic;
using Rain.RTS.Core;
using UnityEngine;

/// <summary>
/// 魔法师攻击策略
/// </summary>
public class MageAttackStrategy : IAttackStrategy
{
    private float speed = 20f;
    // 轨迹
    public static ObjPool trackPool;
    // 爆炸
    public static ObjPool hitEffectPool;
    BaseBattleUnit attacker;

    public void SetTrackPrefabPath(string _path)
    {
        if (trackPool != null)
        {
            trackPool.Clear();
        }
        else
        {
            //PoolMgr.Ins.DestroyPool(trackPool.id);
            trackPool = PoolMgr.Ins.CreatePoolFromPath("FX/LightGlow");
        }
    }

    public void SetHitEffectPrefabPath(string _path)
    {
        if (hitEffectPool != null)
        {
            hitEffectPool.Clear();
        }
        else
        {
            //PoolMgr.Ins.DestroyPool(hitEffectPool.id);
            hitEffectPool = PoolMgr.Ins.CreatePoolFromPath("FX/Hit Fire");
        }
    }

    public void Attack(BaseBattleUnit _attacker, BaseBattleUnit _target, Vector3 _shootPos)
    {
        if (_target == null || _target.IsDead)
            return;
        if (trackPool == null)
            return;

        attacker = _attacker;
        GameObject projectileObj = trackPool.Get();

        ProjectileController projectile = projectileObj.GetComponent<ProjectileController>();
        projectileObj.transform.position = _attacker.bodyPart.shootPos.transform.position;
        if (projectile == null)
        {
            projectile = projectileObj.AddComponent<ProjectileController>();
        }
        projectile.SetCollectAction(SetCollectAction);
        projectile.SetHitAction(SetHitAction);
        projectile.SetHitEffectPool(hitEffectPool);
        projectile.Init(_attacker, _target, speed);
        //Debug.Log($"[{_attacker.UnitName}] 向 [{_target.UnitName}] 发起了魔法攻击");
    }

    public void SetCollectAction(GameObject _go)
    {
        trackPool.CollectOne(_go);
    }

    public void SetHitAction(BaseBattleUnit _unit)
    {
        _unit.Hurt(attacker.Data.attack);
        //Debug.Log($"[{attacker.UnitName}] 的魔法攻击命中了 [{_unit.UnitName}] ,造成 [{attacker.Data.attack}] 点伤害");
    }
}
