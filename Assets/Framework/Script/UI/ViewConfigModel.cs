using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewConfigModel
{
    public string comment;
    public string layer = "NormalUI";
    public bool hasBg = true;
    public bool bgClose = false;
    /// <summary>
    /// 鍏佽瀛樺湪鍦ㄥぇ鍦板浘涓婄殑UI锛屽鏋滃厑璁稿垯锛氬睆骞曡竟缂樺厜鏍囨敼鍙樻牱寮忥紝鎵撳紑椤甸潰涓嶆殏鍋滀笘鐣屾椂闂存祦閫燂紝灞忓箷涓嶈兘绉诲姩鍒癎ame绐楀彛澶栭潰
    /// </summary>
    public bool worldAllow = false;
    /// <summary>
    /// 鎸塃SC閿彲浠ュ叧闂殑椤甸潰
    /// </summary>
    public bool escClose = false;
    public string bgColor;
    public ViewShowMethod showMethod;
}
