using System.Collections;
using System.Collections.Generic;
using Rain.Core;

public class TechData : DBClass
{
    public string techID;
    //�ȼ�
    public DBInt level;
    //״̬ TechState
    public DBInt state;
    //��������ʱ��
    public int levelUpFinishTime;

    public TechData(string _techID)
    {
        techID = _techID;
    }
}
