using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public string effectName = "Slowness";
    public float effectDuration = 5f;
    public float recoverDuration = 5f;
    public float effectValue = 0.5f;

    internal void applyEffect(GameObject player)
    {
        BaseRunner runner = player.GetComponent<BaseRunner>();

        if (runner != null)
        {
            runner.Boost(effectValue, effectDuration);
        }

        Debug.Log(player.name + " has been affected with " + effectName + " effect.");
    }
}