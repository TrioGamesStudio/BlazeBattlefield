using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffectControl : MonoBehaviour
{
    public static CameraEffectControl instance;
    // Start is called before the first frame update
    private Volume volume;

    private Vignette vignette;
    public EyeBlinkEffect EyeBlinkEffect;

    private void Awake()
    {
        instance = this;
        volume = GetComponent<Volume>();
        EyeBlinkEffect = GetComponent<EyeBlinkEffect>();
        volume.profile.TryGet(out vignette);

        HideEyeBlink();
    }

    public void ShowEyeBlink()
    {
        vignette.intensity.value = .5f;
    }
    public void HideEyeBlink()
    {
        vignette.intensity.value = 0;
    }

}
