using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    private CinemachineImpulseSource source;

    void Awake()
    {
        instance = this;
        source = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(EShakeStrenght strength, EShakeShape shape, Vector2 direction = new Vector2())
    {         
        float shakeValue = ShakeStrenghtToFloat(strength);
        if (Random.value > 0.5)
        {
            shakeValue = -shakeValue;
        }
        source.m_ImpulseDefinition.m_ImpulseShape = GetImpulse(shape);

        if (direction == Vector2.zero)
        {
            direction = Vector2.one;
        }
        else
        { 
            direction.Normalize();
        }

        if (Random.value > 0.5)
        {
            source.GenerateImpulse(new Vector2(direction.x * shakeValue,direction.y * shakeValue));
        }
        else
        {
            source.GenerateImpulse(new Vector2(direction.x * shakeValue, direction.y * -shakeValue));
        }
    }

    public static CinemachineImpulseDefinition.ImpulseShapes GetImpulse(EShakeShape shape)
    {
        switch (shape)
        {
            case EShakeShape.bump:
                {
                    return CinemachineImpulseDefinition.ImpulseShapes.Bump;
                }
            case EShakeShape.recoil:
                {
                    return CinemachineImpulseDefinition.ImpulseShapes.Recoil;
                }
            case EShakeShape.rumble:
                {
                    return CinemachineImpulseDefinition.ImpulseShapes.Rumble;
                }
            case EShakeShape.explosion:
                {
                    return CinemachineImpulseDefinition.ImpulseShapes.Explosion;
                }
        }
        return CinemachineImpulseDefinition.ImpulseShapes.Bump;
    }


    public static float ShakeStrenghtToFloat(EShakeStrenght strenght)
    {
        switch (strenght)
        {
            case EShakeStrenght.weak:
                {
                    return 0.03f;
                }
            case EShakeStrenght.medium:
                {
                    return 0.1f;
                }
            case EShakeStrenght.strong:
                {
                    return 0.15f;
                }
            case EShakeStrenght.extraStrong:
                {
                    return 0.3f;
                }
            case EShakeStrenght.cosmic:
                {
                    return 0.45f;
                }
        }
        return 10;
    }
}


public enum EShakeStrenght
{
    weak,
    medium,
    strong,
    extraStrong,
    cosmic
}

public enum EShakeShape
{
    bump,
    recoil,
    explosion,
    rumble
}
