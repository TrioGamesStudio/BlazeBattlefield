using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EyeBlinkEffect : MonoBehaviour
{
    public Volume postProcessingVolume;
    private Vignette vignette;
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
        if (postProcessingVolume.profile.TryGet(out colorAdjustments))
        {
        }
        StartCoroutine(PlayEyeBlinkEffect());
    }
    private IEnumerator PlayEyeBlinkEffect()
    {
        float duration = 1f; // Thời gian nháy mắt
        float elapsed = 0f;

        // Nhắm mắt (tăng độ tối)
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            vignette.intensity.value = Mathf.Lerp(1, 0, progress); // Từ tối hoàn toàn đến mở
            colorAdjustments.saturation.value = Mathf.Lerp(-100, 0, progress); // Phục hồi màu sắc
            yield return null;
        }

        // Đảm bảo giá trị cuối cùng
        vignette.intensity.value = 0;
        colorAdjustments.saturation.value = 0;
    }
}