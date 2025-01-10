using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{
    public static void OnVariableChange<T>(T original, ref T reference, Action<T> callback)
    {
        if (reference == null)
        {
            reference = original;
            return;
        }

        if (!original.Equals(reference))
        {
            callback(original);
            reference = original;
        }
    }
}
