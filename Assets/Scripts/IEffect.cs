using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectArgs
{
    public Vector3 Target;
    public Action Effect;   
}

public interface IEffect
{
    public void ExecuteEffect(EffectArgs args);
}
