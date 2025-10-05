using System.Collections;
using System.Collections.Generic;
using Rain.Core;

public class TechData : DBClass
{
    public string techID;
    //等级
    public DBInt level;
    //状态 TechState
    public DBInt state;
    //升级结束时间
    public int levelUpFinishTime;

    public TechData(string _techID)
    {
        techID = _techID;
    }
}
