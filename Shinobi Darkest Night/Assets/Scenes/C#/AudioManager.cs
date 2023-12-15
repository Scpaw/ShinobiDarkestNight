using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AK.Wwise.Event data;


    private void Start()
    {
        //PlaySound(data);
    }


    public void PlaySound(AK.Wwise.Event JD)
    {
        Debug.Log(JD.Name);
    }
}
