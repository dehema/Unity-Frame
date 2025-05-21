using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TransformExtension
{
    // �ݹ�����Ӷ��󣨰���δ����Ķ���
    public static Transform FindRecursive(this Transform parent, string rootName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == rootName)
                return child;

            // �ݹ�����Ӷ���
            Transform result = child.FindRecursive(rootName);
            if (result != null)
                return result;
        }
        return null;
    }
}