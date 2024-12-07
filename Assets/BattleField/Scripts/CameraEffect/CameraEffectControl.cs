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

    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        instance = this;
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
        vignette.intensity.value = 0;
    }
    [Button]
    public void LerpTest()
    {
        LerpValue lerpValue = new(0, 0.5f,1, (x) =>
        {
            vignette.intensity.value = x;
        });
        LerpValue lerpValue2 = new(1, 0, 3, (x) =>
        {
            Color color = new Color(x, x, x);
            colorAdjustments.colorFilter.value = color;
        });
    }
}
