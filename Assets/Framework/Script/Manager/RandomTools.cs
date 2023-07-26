using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RandomTools
{
    /// <summary>
    /// List打乱顺序
    /// </summary>
    /// <param name="chance"></param>
    /// <returns></returns>
    public static List<T> RandomList<T>(List<T> list, System.Random _random = null)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int index;
            if (_random != null)
                index = _random.Next(0, list.Count);
            else
                index = Random.Range(0, list.Count);
            if (index != i)
            {
                var temp = list[i];
                list[i] = list[index];
                list[index] = temp;
            }
        }
        return list;
    }

    /// <summary>
    /// 随机一个数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <returns></returns>
    public static T GetVal<T>(List<T> _list)
    {
        if (_list.Count == 0)
        {
            return default(T);
        }
        while (true)
        {
            foreach (var item in _list)
            {
                if (Random.Range(0f, _list.Count) < 1)
                {
                    return item;
                }
            }
        }
    }

    /// <summary>
    /// 带权随机
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_options"></param>
    /// <param name="_weights"></param>
    /// <returns></returns>
    public static T GetWeightRandomIndex<T>(List<T> _options, List<int> _weights)
    {
        List<int> indexs = new List<int>();
        int optionIndex = 0;
        foreach (var option in _options)
        {
            foreach (var weight in _weights)
            {
                for (int j = 0; j < weight; j++)
                {
                    indexs.Add(optionIndex);
                }
            }
            optionIndex++;
        }
        int randomIndex = indexs[Random.Range(0, indexs.Count)];
        return _options[randomIndex];
    }
}
