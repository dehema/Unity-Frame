using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 消息传递的参数
/// </summary>
public class MessageData
{
    /*
     *  1.创建独立的消息传递数据结构，而不使用object，是为了避免数据传递时的类型强转
     *  2.制作过程中遇到实际需要传递的数据类型，在这里定义即可
     *  3.实际项目中需要传递参数的类型其实并没有很多种，这种方式基本可以满足需求
     */
    public bool valueBool;
    public bool valueBool2;
    public int valueInt;
    public int valueInt2;
    public int valueInt3;
    public float valueFloat;
    public float valueFloat2;
    public double valueDouble;
    public double valueDouble2;
    public string valueString;
    public string valueString2;
    public GameObject valueGameObject;
    public GameObject valueGameObject2;
    public GameObject valueGameObject3;
    public GameObject valueGameObject4;
    public Transform valueTransform;
    public List<string> valueStringList;
    public List<Vector2> valueVec2List;
    public List<int> valueIntList;
    public System.Action messageCallBack;
    public Vector2 vec2_1;
    public Vector2 vec2_2;
    public MessageData()
    {
    }
    public MessageData(Vector2 v2_1)
    {
        vec2_1 = v2_1;
    }
    public MessageData(Vector2 v2_1, Vector2 v2_2)
    {
        vec2_1 = v2_1;
        vec2_2 = v2_2;
    }
    /// <summary>
    /// 创建一个带bool类型的数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public MessageData(bool value)
    {
        valueBool = value;
    }
    public MessageData(bool value, bool value2)
    {
        valueBool = value;
        valueBool2 = value2;
    }
    /// <summary>
    /// 创建一个带int类型的数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public MessageData(int value)
    {
        valueInt = value;
    }
    public MessageData(int value, int value2)
    {
        valueInt = value;
        valueInt2 = value2;
    }
    public MessageData(int value, int value2, int value3)
    {
        valueInt = value;
        valueInt2 = value2;
        valueInt3 = value3;
    }
    public MessageData(List<int> value,List<Vector2> value2)
    {
        valueIntList = value;
        valueVec2List = value2;
    }
    /// <summary>
    /// 创建一个带float类型的数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public MessageData(float value)
    {
        valueFloat = value;
    }
    public MessageData(float value,float value2)
    {
        valueFloat = value;
        valueFloat = value2;
    }
    /// <summary>
    /// 创建一个带double类型的数据
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public MessageData(double value)
    {
        valueDouble = value;
    }
    public MessageData(double value, double value2)
    {
        valueDouble = value;
        valueDouble = value2;
    }
    /// <summary>
    /// 创建一个带string类型的数据
    /// </summary>
    /// <param name="value"></param>
    public MessageData(string value)
    {
        valueString = value;
    }
    /// <summary>
    /// 创建两个带string类型的数据
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    public MessageData(string value1,string value2)
    {
        valueString = value1;
        valueString2 = value2;
    }
    public MessageData(GameObject value1)
    {
        valueGameObject = value1;
    }

    public MessageData(Transform transform)
    {
        valueTransform = transform;
    }
}

