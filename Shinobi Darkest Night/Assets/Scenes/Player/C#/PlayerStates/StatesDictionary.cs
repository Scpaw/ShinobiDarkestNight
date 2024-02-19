using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class StateDictionary : SerializableDictionary<string, DirectionalAnimation>
{
    public AnimationClip GetFacingClipFromState(string indexString, Vector2 facingDirection)
    {
        if (TryGetValue(indexString, out DirectionalAnimation animationSet))
        {
            return animationSet.GetFacingClip(facingDirection);
        }
        else
        {
            Debug.LogError($"Character State {indexString} is not found in the StateAnimations Dictionary");
        }
    
        return null;
    }
}
