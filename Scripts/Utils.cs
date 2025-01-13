using System;
using UnityEngine;

public static class Utils 
{
    /// <summary> Listens the change of a variable and sets the reference with the new value. </summary>
    public static void OnVariableChange<T>(T original, ref T reference, Action callback = null)
    {
        if (reference == null)
        {
            reference = original;
            return;
        }

        if (!original.Equals(reference))
        {
            reference = original;
            callback?.Invoke();
        }
    }

    /// <summary> Converts from a 0 to 100 value to an AudioMixer-Compatible one. </summary>
    public static float ParseVolume(float volume, AnimationCurve curve)
    {
        if (volume >= 0 && volume <= 100)
        {
            return (curve.Evaluate(volume / 100) * 80) - 80;
        }
        throw new Exception($"Volume was outside bounds. Value recieved: {volume}");
    }

    /// <summary> Gets a PlayerComponent reference using the tag "Player". </summary>
    public static PlayerController GetPlayer()
    {
        return GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
}
