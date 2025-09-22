using System.Collections.Generic;
using Rain.Core;

public class PlayerData : DBClass
{
    public DBInt gold;
    public DBString playerName;
    public DBInt level;
    public DBInt exp;

    public DBFloat hp;
    public DBFloat hpMax;

    public List<string> techs = new List<string>();
}
