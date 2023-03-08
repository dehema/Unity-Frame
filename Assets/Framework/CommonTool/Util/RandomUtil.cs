using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtil
{
    /// <summary>
    /// 带权随机
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    public static T GetWeightRandom<T>(T[] objs, int[] weights)
    {
        int randomIndex = GetWeightRandomIndex(objs, weights);
        return objs[randomIndex];
    }

    public static int GetWeightRandomIndex<T>(T[] objs, int[] weights)
    {
        List<int> indexes = new List<int>();
        int totalWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if (i >= objs.Length)
            {
                break;
            }
            int weight = weights[i];
            for (int j = 0; j < weight; j++)
            {
                indexes.Add(i);
            }
            totalWeight += weight;
        }

        int randomIndex = Random.Range(0, totalWeight);
        return indexes[randomIndex];
    }

    public static int GetWeightRandomIndex<T>(Dictionary<T, int> dict)
    {
        T[] keys = new T[dict.Count];
        int[] values = new int[dict.Count];
        dict.Keys.CopyTo(keys, 0);
        dict.Values.CopyTo(values, 0);
        return GetWeightRandomIndex(keys, values);
    }

    public static T GetWeightRandom<T>(Dictionary<T, int> dict)
    {
        T[] keys = new T[dict.Count];
        int[] values = new int[dict.Count];
        dict.Keys.CopyTo(keys, 0);
        dict.Values.CopyTo(values, 0);
        return GetWeightRandom(keys, values);
    }

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

    public static bool InChance(double chance)
    {
        return Random.Range(0, 100) <= chance * 100;
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
}
