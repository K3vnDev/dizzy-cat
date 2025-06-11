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
    public static float ParseVolume(float volume)
    {
        if (volume >= 0 && volume <= 100)
        {
            return (GameManager.I.soundCurve.Evaluate(volume / 100) * 80) - 80;
        }
        throw new Exception($"Volume was outside bounds. Value recieved: {volume}");
    }

    /// <summary> Finds a PlayerComponent reference in the current scene. </summary>
    public static PlayerController GetPlayer()
    {
        return GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    /// <summary> 
    /// Parses a raw input value into a controlled one with a threshold. <br/>
    /// Each axis of the returned Vector2 can be either 1, 0 or -1.
    /// </summary>
    public static Vector2 ParseRawInput(Vector2 rawInput, float threshold = 0.75f)
    {
        float ParseAxis(float value)
        {
            if (Mathf.Abs(value) < threshold) return 0;
            else return Mathf.Sign(value);
        }
        return new Vector2(ParseAxis(rawInput.x), ParseAxis(rawInput.y));
    }
}
