using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildToolBase
{
    public bool isWindowExpanded = true;     //�����Ƿ�չ��
    public BuildToolConfig config;

    public BuildToolBase(BuildToolConfig _config)
    {
        config = _config;
    }
}
