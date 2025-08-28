using System.Collections;
using System.Collections.Generic;
using Rain.RTS.Core;
using UnityEngine;

public class MageAttackStrategy : MonoBehaviour
{
    private float speed = 10f;
    public static ObjPool trackPool;
    public static ObjPool explodePool;
    BaseBattleUnit attacker;

    public void SetProjectilePrefabPath(string _arrowPath)
    {
        if (trackPool != null)
        {
            trackPool.Clear();
        }
        trackPool = PoolMgr.Ins.CreatePoolFromPath("FX/JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Light/CFXR3 LightGlow A (Loop)");
        if (explodePool != null)
        {
            explodePool.Clear();
        }
        explodePool = PoolMgr.Ins.CreatePoolFromPath("FX/JMO Assets/Cartoon FX Remaster/CFXR Prefabs/Light/CFXR3 Hit Fire B");
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
        projectile.Init(_attacker, _target, speed);
        Debug.Log($"[{_attacker.UnitName}] 向 [{_target.UnitName}] 射箭");
    }

    public void SetCollectAction(GameObject _go)
    {
        trackPool.CollectOne(_go);
    }

    public void SetHitAction(BaseBattleUnit _unit)
    {
        _unit.Hurt(attacker.Data.attack);
        Debug.Log($"[{attacker.UnitName}] 的箭命中了 [{_unit.UnitName}] ,造成 [{attacker.Data.attack}] 点伤害");
    }
}
