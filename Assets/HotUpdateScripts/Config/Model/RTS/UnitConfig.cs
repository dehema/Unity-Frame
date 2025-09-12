using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rain.Core
{
    public sealed partial class UnitConfig : Luban.BeanBase
    {
        /// <summary>
        /// ÕóÓª
        /// </summary>
        public Faction faction = Faction.Player;

        public UnitConfig() { }
        public UnitConfig(UnitConfig _other)
        {
            this.ID = _other.ID;
            this.UnitType = _other.UnitType;
            this.Name = _other.Name;
            this.Hp = _other.Hp;
            this.MoveSpeed = _other.MoveSpeed;
            this.Attack = _other.Attack;
            this.AttackInterval = _other.AttackInterval;
            this.AttackRange = _other.AttackRange;
            this.AttackHurtTime = _other.AttackHurtTime;
            this.AttackDuration = _other.AttackDuration;
            this.AngularSpeed = _other.AngularSpeed;
            this.Height = _other.Height;
            this.Value = _other.Value;
            this.ValuePre = _other.ValuePre;
            this.Upkeep = _other.Upkeep;
            this.FullID = _other.FullID;
        }
    }
}