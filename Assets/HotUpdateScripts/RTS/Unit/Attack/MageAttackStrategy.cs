using System.Collections;
using System.Collections.Generic;
using Rain.RTS.Core;
using UnityEngine;

/// <summary>
/// ħ��ʦ��������
/// </summary>
public class MageAttackStrategy : IAttackStrategy
{
    private float speed = 20f;
    // �켣
    public static ObjPool trackPool;
    // ��ը
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
        //Debug.Log($"[{_attacker.UnitName}] �� [{_target.UnitName}] ������ħ������");
    }

    public void SetCollectAction(GameObject _go)
    {
        trackPool.CollectOne(_go);
    }

    public void SetHitAction(BaseBattleUnit _unit)
    {
        _unit.Hurt(attacker.Data.attack);
        //Debug.Log($"[{attacker.UnitName}] ��ħ������������ [{_unit.UnitName}] ,��� [{attacker.Data.attack}] ���˺�");
    }
}
