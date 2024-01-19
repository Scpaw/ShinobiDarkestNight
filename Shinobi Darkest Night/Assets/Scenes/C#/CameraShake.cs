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

    public void Shake()
    {
        float shakeValue = 0.05f;
        if (Random.value > 0.5)
        {
            shakeValue = -shakeValue;
        }

        if (Random.value > 0.5)
        {
            source.GenerateImpulse(new Vector2(shakeValue, shakeValue));
        }
        else
        {
            source.GenerateImpulse(new Vector2(shakeValue, -shakeValue));
        }

        Debug.Log("Shake");
    }
    
}
