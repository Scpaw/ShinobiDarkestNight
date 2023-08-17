using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript instance;
    private CinemachineVirtualCamera cam;

    private void Awake()
    {
        instance = this;
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public CinemachineVirtualCamera GetCam() { return cam; }
}
