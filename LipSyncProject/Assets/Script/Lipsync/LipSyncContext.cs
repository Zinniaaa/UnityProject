using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LipSyncContext : OVRLipSyncContextBase
{
    // Start is called before the first frame update
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        ProcessAudioSamples(data, channels);
    }

    public void ProcessAudioSamples(float[] data, int channels)
    {
        if ((LipSync.IsInitialized() != LipSync.Result.Success) || audioSource == null)
        {
            return;
        }

        if (Context == 0 || LipSync.IsInitialized() != LipSync.Result.Success)
        {
            return;
        }

        var frame = this.Frame;
        LipSync.ProcessFrame(Context, data, frame, channels == 2);


    }
}
