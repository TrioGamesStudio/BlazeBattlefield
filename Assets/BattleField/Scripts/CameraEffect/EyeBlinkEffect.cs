using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EyeBlinkEffect : MonoBehaviour
{
    public Volume postProcessingVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;
    private ColorAdjustments colorAdjustments;
    private void Start()
    {

    }
    [Button]
    private void Test()
    {
        if (postProcessingVolume.profile.TryGet(out vignette))
        {
        }
        if (postProcessingVolume.profile.TryGet(out depthOfField))
        {
        }
        if (postProcessingVolume.profile.TryGet(out colorAdjustments))
        {
        }
    }

}
