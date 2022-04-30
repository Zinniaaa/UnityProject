using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OVRLipSyncContextBase : MonoBehaviour
{
    public AudioSource audioSource = null;

    public LipSync.ContextProviders provider = LipSync.ContextProviders.Enhanced;

    public bool enableAcceleration = true;

    public LipSync.Result result = LipSync.Result.Unknown;

    private LipSync.Frame frame = new LipSync.Frame();
    private uint context = 0;    // 0 is no context

    private int _smoothing;
    public int Smoothing
    {
        set
        {
            LipSync.Result result = LipSync.SendSignal(context, LipSync.Signals.VisemeSmoothing, value, 0);

            if (result != LipSync.Result.Success)
            {
                if (result == LipSync.Result.InvalidParam)
                {
                    Debug.LogError("OVRLipSyncContextBase.SetSmoothing: A viseme smoothing parameter is invalid, it should be between 1 and 100!");
                }
                else
                {
                    Debug.LogError("OVRLipSyncContextBase.SetSmoothing: An unexpected error occured.");
                }
            }

            _smoothing = value;
        }
        get
        {
            return _smoothing;
        }
    }

    public uint Context
    {
        get
        {
            return context;
        }
    }

    protected LipSync.Frame Frame
    {
        get
        {
            return frame;
        }
    }

    void Awake()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
        result = LipSync.CreateContext(ref context, provider, 0, enableAcceleration);
    }


    void OnDestroy()
    {

        if (context != 0)
        {
            if (LipSync.DestroyContext(context) != LipSync.Result.Success)
            {
                Debug.LogError("OVRLipSyncContextBase.OnDestroy ERROR: Could not delete Phoneme context.");
            }
        }
    }

    public LipSync.Frame GetCurrentPhonemeFrame()
    {
        return frame;
    }

    public void SetVisemeBlend(int viseme, int amount)
    {
        LipSync.Result result = LipSync.SendSignal(context, LipSync.Signals.VisemeAmount, viseme, amount);

        if (result != LipSync.Result.Success)
        {
            if (result == LipSync.Result.InvalidParam)
            {
                Debug.LogError("OVRLipSyncContextBase.SetVisemeBlend: Viseme ID is invalid.");
            }
            else
            {
                Debug.LogError("OVRLipSyncContextBase.SetVisemeBlend: An unexpected error occured.");
            }
        }
    }

    public void SetLaughterBlend(int amount)
    {
        LipSync.Result result = LipSync.SendSignal(context, LipSync.Signals.LaughterAmount, amount, 0);

        if (result != LipSync.Result.Success)
        {
            Debug.LogError("OVRLipSyncContextBase.SetLaughterBlend: An unexpected error occured.");
        }
    }

    public LipSync.Result ResetContext()
    {
        frame.Reset();
        return LipSync.ResetContext(context);
    }
}
