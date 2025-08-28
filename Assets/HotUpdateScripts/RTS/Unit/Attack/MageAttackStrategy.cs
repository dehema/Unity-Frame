using System.Collections;
using System.Collections.Generic;
using Rain.RTS.Core;
using UnityEngine;

/// <summary>
/// ħ��ʦ��������
/// </summary>
public class MageAttackStrategy : IAttackStrategy
{
    private float speed = 10f;
    // �켣
    public static ObjPool trackPool;
    // ��ը
    public static ObjPool hitEffectPool;
    BaseBattleUnit attacker;

    public void SetProjectilePrefabPath(string _arrowPath)
    {
        if (trackPool != null)
        {
            trackPool.Clear();
        }
        trackPool = PoolMgr.Ins.CreatePoolFromPath("FX/JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Light/CFXR3 LightGlow A (Loop)");
        if (hitEffectPool != null)
        {
            hitEffectPool.Clear();
        }
        hitEffectPool = PoolMgr.Ins.CreatePoolFromPath("FX/JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Light/CFXR3 Hit Fire B");
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
        Debug.Log($"[{_attacker.UnitName}] �� [{_target.UnitName}] ������ħ������");
    }

    public void SetCollectAction(GameObject _go)
    {
        trackPool.CollectOne(_go);
    }

    public void SetHitAction(BaseBattleUnit _unit)
    {
        _unit.Hurt(attacker.Data.attack);
        Debug.Log($"[{attacker.UnitName}] ��ħ������������ [{_unit.UnitName}] ,��� [{attacker.Data.attack}] ���˺�");
    }
}
