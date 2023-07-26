using DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : DBClass
{
    public DBInt gold;
    public DBString playerName;
    public DBInt level;
    public DBInt exp;

    public DBFloat hp;
    public DBFloat hpMax;
}