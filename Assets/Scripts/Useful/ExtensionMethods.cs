using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

    public static float Remap(this float t, float a, float b, float c, float d)
    {
        return (t - a) * ((d - c) / (b - a)) + c;
    }

    public static void PrintList<T>(this List<T> l)
    {
        foreach(var item in l)
        {
            Debug.Log(item);
        }
    }

}