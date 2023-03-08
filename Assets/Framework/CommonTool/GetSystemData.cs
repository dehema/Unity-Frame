using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using UnityEditor;

public static class MergeDictionaries
{
    public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
    {
        if (second == null || first == null) return;
        foreach (var item in second)
            if (!first.ContainsKey(item.Key))
                first.Add(item.Key, item.Value);
    }
}

public class GetSystemData : MonoSingleton<GetSystemData>
{
    // Start is called before the first frame update
    public bool NeedDelete;
    public bool isTest;

    static public GetSystemData instance;
    protected void Awake()
    {
        Application.targetFrameRate = 60;
        if (NeedDelete)
        {
            PlayerPrefs.DeleteAll();
        }
        instance = this;
    }


    public float getCameraWidth()
    {
        return Camera.main.ViewportToWorldPoint(new Vector2(1, 0)).x - Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x;
    }
    public float getCameraHeight()
    {
        return Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y - Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).y;
    }
    /// <summary>
    /// 获取世界坐标
    /// </summary>
    /// <param name="gameobj"></param>
    /// <returns></returns>
    public Vector3 getWorldPoint(GameObject gameobj)
    {
        Transform parent = gameobj.transform.parent;
        //结点偏差
        Vector3 offset = new Vector3(gameobj.transform.localPosition.x * parent.transform.localScale.x,
                                     gameobj.transform.localPosition.y * parent.transform.localScale.y,
                                     gameobj.transform.localPosition.z * parent.transform.localScale.z);
        //子节点世界坐标

        Vector3 worldPos = parent.position + parent.rotation * offset;
        return worldPos;
    }
    /// <summary>
    /// 获取最近物体
    /// </summary>
    /// <param name="listTemp"></param>
    /// <returns></returns>
    public GameObject GetNearestGameObject(Transform SelfTransform, List<Transform> listTemp)
    {
        if (listTemp != null && listTemp.Count > 0)
        {
            Transform targetTemp = listTemp.Count > 0 ? listTemp[0] : null;
            float dis = Vector3.Distance(SelfTransform.position, listTemp[0].position);
            float disTemp;
            for (int i = 1; i < listTemp.Count; i++)
            {
                disTemp = Vector3.Distance(SelfTransform.position, listTemp[i].position);
                if (disTemp < dis)
                {
                    targetTemp = listTemp[i];
                    dis = disTemp;
                }
            }
            return targetTemp.gameObject;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 两个点的角度
    /// 0°正右
    /// 90°正上
    /// -90°正下
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public float PointToAngle(Vector2 p1, Vector2 p2)
    {
        Vector2 p;
        p.x = p2.x - p1.x;
        p.y = p2.y - p1.y;
        return Mathf.Atan2(p.y, p.x) * 180 / Mathf.PI;
    }
    /// <summary>
    /// -180-180 转为 0-360
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public float Angle180To360(float angle)
    {
        if (angle >= 0 && angle <= 180)
            return angle;
        else
            return 360 + angle;
    }
    /// <summary>
    /// 返回Unity的角度
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public int GetUnityDirection(Vector2 p1, Vector2 p2)
    {
        float angle = Angle180To360(PointToAngle(p1, p2));
        float temp = 360 * 0.125f;//分为8个方向
        float dir = 0;
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            if (angle >= (i * temp) - (temp * 0.5f) && angle < (i * temp) + (temp * 0.5f))
            {
                dir = i * temp;
                index = i;
                break;
            }
        }
        return index;
    }

    /// <summary>
    /// 获取图片真实大小
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Vector2 getTextureTrueSize(Renderer Renderer)
    {
        float x = Renderer.bounds.extents.x * 2 / Renderer.transform.lossyScale.x;
        float y = Renderer.bounds.extents.y * 2 / Renderer.transform.lossyScale.y;
        return new Vector2(x, y);
    }
    public Vector2 getTextureTrueSize(Image image)
    {
        float x = image.sprite.textureRect.width;
        float y = image.sprite.textureRect.height;
        return new Vector2(x, y);
    }
    /// <summary>
    /// 获取Sprite大小
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Vector2 getSpriteSize(GameObject obj)
    {
        float x = obj.GetComponent<SpriteRenderer>().bounds.extents.x * 2;
        float y = obj.GetComponent<SpriteRenderer>().bounds.extents.y * 2;
        return new Vector2(x, y);
    }
    /// <summary>
    /// 获取鼠标点击位置
    /// </summary>
    /// <returns></returns>
    public Vector2 getTouchPosition()
    {

        Vector3 v = Input.mousePosition;
        float xa = v.x / Screen.width;
        float ya = v.y / Screen.height;
        float screenW = getCameraWidth();
        float screenH = getCameraHeight();
        float x = -screenW / 2 + xa * screenW;
        float y = -screenH / 2 + ya * screenH;
        return new Vector2(x, y);
    }

    public int getWeightIndex(List<string> weightList)
    {
        float sumWeight = 0;
        foreach (string weight in weightList)
        {
            float w = float.Parse(weight);
            sumWeight += w;
        }
        float r = UnityEngine.Random.Range(0, sumWeight);
        float now_weight = 0;
        int index = -1;
        for (int i = 0; i < weightList.Count; i++)
        {
            now_weight += float.Parse(weightList[i]);
            if (r < now_weight)
            {
                index = i;
                break;
            }
        }
        return index;

    }

    public int[] GetRandomSequence(int total, int count)
    {
        int[] sequence = new int[total];
        int[] output = new int[count];

        for (int i = 0; i < total; i++)
        {
            sequence[i] = i;
        }
        int end = total - 1;
        for (int i = 0; i < count; i++)
        {
            //随机一个数，每随机一次，随机区间-1
            int num = UnityEngine.Random.Range(0, end + 1);
            output[i] = sequence[num];
            //将区间最后一个数赋值到取到的数上
            sequence[num] = sequence[end];
            end--;
        }
        return output;
    }
    public Dictionary<string, object> StringToDictionary(string value)
    {
        if (value.Length < 1)
        {
            return null;
        }
        //Console.Write("1777value--" + value);
        Dictionary<string, object> dic = new Dictionary<string, object>();

        string[] dicStrs = value.Split('|');
        foreach (string str in dicStrs)
        {
            //    Console.Write("183value--" + str);
            string[] strs = str.Split('=');
            dic.Add(strs[0], strs[1]);
        }
        return dic;
    }

    public string DictionaryToString(Dictionary<string, object> Info)
    {

        string str = "";

        Dictionary<string, object> dic = Info;
        foreach (string key in dic.Keys)
        {
            str += (key + "=" + dic[key]);
            str += "|";
        }
        str = str.Substring(0, str.Length - 1);
        return str;
    }


    public int DayDateDiff(string dateStart, DateTime dateEnd)
    {
        DateTime start = Convert.ToDateTime(Convert.ToDateTime(dateStart).ToShortDateString());
        DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());
        // 计算时间间隔
        TimeSpan sp = end.Subtract(start);
        return sp.Days;
    }

    public int SecDateDiff(string dateStart, DateTime dateEnd)
    {
        DateTime start = Convert.ToDateTime(Convert.ToDateTime(dateStart));
        DateTime end = dateEnd;
        // 计算时间间隔
        TimeSpan sp = end.Subtract(start);
        return (int)sp.TotalSeconds;
    }
    public TimeSpan DateDiff(DateTime dateStart, DateTime dateEnd)
    {
        DateTime start = dateStart;
        DateTime end = dateEnd;
        // 计算时间间隔
        TimeSpan sp = end.Subtract(start);
        return sp;
    }

    //public string getObjDirection(Transform trans1,Transform trans2)
    //{
    //    if (trans)
    //}

    public Vector3 GetPosAtText(Canvas canvas, Text text, string strFragment)
    {
        int strFragmentIndex = text.text.IndexOf(strFragment);//-1表示不包含strFragment
        Vector3 stringPos = Vector3.zero;
        if (strFragmentIndex > -1)
        {
            Vector3 firstPos = GetPosAtText(canvas, text, strFragmentIndex);
            Vector3 lastPos = GetPosAtText(canvas, text, strFragmentIndex + strFragment.Length);
            stringPos = (firstPos + lastPos) * 0.5f;
        }
        else
        {
            stringPos = GetPosAtText(canvas, text, strFragmentIndex);
        }
        return stringPos;
    }

    public Vector3 GetPosAtText(Canvas canvas, Text text, int charIndex)
    {
        string textStr = text.text;
        Vector3 charPos = Vector3.zero;
        if (charIndex <= textStr.Length && charIndex > 0)
        {
            TextGenerator textGen = new TextGenerator(textStr.Length);
            Vector2 extents = text.gameObject.GetComponent<RectTransform>().rect.size;
            textGen.Populate(textStr, text.GetGenerationSettings(extents));

            int newLine = textStr.Substring(0, charIndex).Split('\n').Length - 1;
            int whiteSpace = textStr.Substring(0, charIndex).Split(' ').Length - 1;
            int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - (whiteSpace * 4);
            if (indexOfTextQuad < textGen.vertexCount)
            {
                charPos = (textGen.verts[indexOfTextQuad].position +
                           textGen.verts[indexOfTextQuad + 1].position +
                           textGen.verts[indexOfTextQuad + 2].position +
                           textGen.verts[indexOfTextQuad + 3].position) / 4f;


            }
        }
        charPos /= canvas.scaleFactor;//适应不同分辨率的屏幕
        charPos = text.transform.TransformPoint(charPos);//转换为世界坐标
        charPos = new Vector3(charPos.x + 0.005f, charPos.y + 0.005f, 0);
        return charPos;
    }
    /// <summary>
    /// 获取点到线的距离
    /// </summary>
    /// <param name="pB"></param>
    /// <param name="pA1"></param>
    /// <param name="pA2"></param>
    /// <returns></returns>
    public float DistanceByArea(Vector3 pB, Vector3 pA1, Vector3 pA2)
    {
        Vector3 a2A1 = pA1 - pA2;
        Vector3 a2B = pB - pA2;

        Vector3 normal = Vector3.Cross(a2A1, a2B);
        Vector3 a2A1Temp = Vector3.Cross(a2A1, normal).normalized;

        return Mathf.Abs(Vector3.Dot(a2B, a2A1Temp));
    }




    #region  字符串加密解密

    private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    /// <summary>
    /// DES加密字符串
    /// </summary>
    /// <param name="encryptString">待加密的字符串</param>
    /// <param name="key">加密密钥,要求为8位</param>
    /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
    public static string EncryptDES(string encryptString, string key = "abcdefgh")
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(Application.identifier.Substring(0, 8));
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            cStream.Close();
            return Convert.ToBase64String(mStream.ToArray());
        }
        catch
        {
            //Debug.LogError("StringEncrypt/EncryptDES()/ Encrypt error!");
            return encryptString;
        }
    }

    /// <summary>
    /// DES解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <param name="key">解密密钥,要求为8位,和加密密钥相同</param>
    /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
    public static string DecryptDES(string decryptString, string key = "13717421")
    {
        byte[] rgbKey = Encoding.UTF8.GetBytes(Application.identifier.Substring(0, 8));
        byte[] rgbIV = Keys;
        byte[] inputByteArray = Convert.FromBase64String(decryptString);
        DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
        MemoryStream mStream = new MemoryStream();
        CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
        cStream.Write(inputByteArray, 0, inputByteArray.Length);
        cStream.FlushFinalBlock();
        cStream.Close();
        return Encoding.UTF8.GetString(mStream.ToArray());
    }

    #endregion

}
