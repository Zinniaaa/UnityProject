using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LipSyncContextMorphTarget : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer = null;

    public int[] visemeToBlendTargets = Enumerable.Range(0, LipSync.VisemeCount).ToArray();

    public int smoothAmount = 70;

    private OVRLipSyncContextBase lipsyncContext = null;

    void Start()
    {
        lipsyncContext = GetComponent<OVRLipSyncContextBase>();
        lipsyncContext.Smoothing = smoothAmount;
    }

    void Update()
    {
        if ((lipsyncContext != null) && (skinnedMeshRenderer != null))
        {
            LipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
            if (frame != null)
            {
                SetVisemeToMorphTarget(frame);
            }

            if (smoothAmount != lipsyncContext.Smoothing)
            {
                lipsyncContext.Smoothing = smoothAmount;
            }
        }
    }

    void SetVisemeToMorphTarget(LipSync.Frame frame)
    {
        for (int i = 0; i < visemeToBlendTargets.Length; i++)
        {
            if (visemeToBlendTargets[i] != -1)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(visemeToBlendTargets[i], frame.Visemes[i] * 100.0f);
            }
        }
    }
}
