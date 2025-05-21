using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TransformExtension
{
    // 递归查找子对象（包括未激活的对象）
    public static Transform FindRecursive(this Transform parent, string rootName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == rootName)
                return child;

            // 递归查找子对象
            Transform result = child.FindRecursive(rootName);
            if (result != null)
                return result;
        }
        return null;
    }
}