using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPrint
{
    public static void Print(params object[] args)
    {
        if (args == null)
        {
            Debug.Log("Array is Null");
        }
        if (args.Length == 0)
        {
            Debug.Log("Array is Empty");
        }

        string[] strArgs = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            strArgs[i] = args[i].ToString();
        }
        Debug.Log(String.Join("  ", strArgs));
    }
}
