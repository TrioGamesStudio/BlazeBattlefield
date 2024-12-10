using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffectControl : MonoBehaviour
{
    public static CameraEffectControl instance;
    // Start is called before the first frame update
    private Volume volume;

    public Vignette vignette;
    public ColorAdjustments colorAdjustments;
    public EyeBlinkEffect EyeBlinkEffect;

    private void Awake()
    {
        instance = this;

        EyeBlinkEffect = GetComponent<EyeBlinkEffect>();

        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);
        HideEyeBlink();
    }

    public void ShowEyeBlink()
    {
        vignette.intensity.value = .5f;
    }
    public void HideEyeBlink()
    {
        vignette.intensity.value = .2f;
    }
}
