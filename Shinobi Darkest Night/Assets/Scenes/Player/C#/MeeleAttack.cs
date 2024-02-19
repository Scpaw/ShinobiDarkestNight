using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeeleAttack
{
    public DirectionalAnimation animation;
    public float dmg;
    public int shurikenDrop;
    public float range;
    [Range(0f,1f)]
    public float movingDuration;

    public AnimationClip GetFacingAnimation(Vector2 facingDirection)
    {
        return animation.GetFacingClip(facingDirection);
    }

}
