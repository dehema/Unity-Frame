using UnityEngine;
using UnityEngine.UI;

public class Lang : MonoBehaviour
{
    public string tid;
    void Start()
    {
        if (string.IsNullOrEmpty(tid))
        {
            Debug.LogError("tid为空", gameObject);
        }
        Refresh();
    }

    public void Refresh()
    {
        Text txt = GetComponent<Text>();
        txt.text = LangMgr.Ins.Get(tid);
    }
}
