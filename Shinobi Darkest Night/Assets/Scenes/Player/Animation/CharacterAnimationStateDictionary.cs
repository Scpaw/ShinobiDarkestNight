using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CharacterAnimationStateDictionary : SerializableDictionary<CharacterState, DirectionalAnimation>
{
    public AnimationClip GetFacingClipFromState(CharacterState characterState, Vector2 facingDirection)
    {
        if (TryGetValue(characterState, out DirectionalAnimation animationSet))
        {
            return animationSet.GetFacingClip(facingDirection);
        }
        else
        {
            Debug.LogError($"Character State {characterState.name} is not found in the StateAnimations Dictionary");
        }

        return null;
    }
}

