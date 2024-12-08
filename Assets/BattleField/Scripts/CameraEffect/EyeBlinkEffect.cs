using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EyeBlinkEffect : MonoBehaviour
{
    [Header("Open Eye settings")]
    [SerializeField, Range(0, 1)] private float _openStartValue = 0.5f;
    [SerializeField, Range(0, 1)] private float _openEndValue = 0;
    [SerializeField, Min(0)] private float _openTime = 1;
    [SerializeField, Range(0, 1)] private float _openStartColorValue = 0;
    [SerializeField, Range(0, 1)] private float _openEndColorValue = 1;
    [SerializeField, Min(0)] private float _openColorTime = 3;

    [Header("Close Eye settings")]
    [SerializeField, Range(0, 1)] private float _closeStartValue = 0;
    [SerializeField, Range(0, 1)] private float _closeEndValue = 0.5f;
    [SerializeField, Min(0)] private float _closeTime = 1;
    [SerializeField, Range(0, 1)] private float _closeStartColorValue = 1;
    [SerializeField, Range(0, 1)] private float _closeEndColorValue = 0;
    [SerializeField, Min(0)] private float _closeColorTime = 3;

    private LerpValue vignette;
    private LerpValue color;
    private LerpValue canvasGroup;

    [Button]
    public void CloseEyeImmediately()
    {
        VignetteLerp(_closeStartValue, _closeEndValue, 0);
        ColorFiler(_closeStartColorValue, _closeEndColorValue, 0);
        CanvasGroup(1, 0, 0);
    }
    [Button]
    public void OpenEyeImmediately()
    {
        StartCoroutine(openCoroutine());
    }
    private IEnumerator openCoroutine()
    {

        // sau khi tinh day
        ColorFiler(0, 1, .7f);
        yield return new WaitForSeconds(.7f);
        VignetteLerp(.7f, .2f, 2);
        CanvasGroup(0, 1, 2);
    }
    

    [Button]
    public void OpenEye()
    {
        VignetteLerp(_openStartValue, _openEndValue, _openTime);
        ColorFiler(_openStartColorValue, _openEndColorValue, _openColorTime);
        CanvasGroup(0, 1, _openColorTime);
    }

    [Button]
    public void CloseEye()
    {
        VignetteLerp(_closeStartValue, _closeEndValue, _closeTime);
        ColorFiler(_closeStartColorValue, _closeEndColorValue, _closeColorTime);
        CanvasGroup(1, 0, _closeColorTime);
    }
  
    private void VignetteLerp(float startVignette, float endVignette, float vignetteTime)
    {
        vignette = InitializeOrUpdateLerpValue(vignette, startVignette, endVignette, vignetteTime, (x) =>
        {
            CameraEffectControl.instance.vignette.intensity.value = x;
        });
    }

    private void ColorFiler(float startColorValue, float endColorValue, float colorValueTime)
    {
        color = InitializeOrUpdateLerpValue(color, startColorValue, endColorValue, colorValueTime, (x) =>
        {
            Color color = new Color(x, x, x);
            CameraEffectControl.instance.colorAdjustments.colorFilter.value = color;
        });
    }

    private void CanvasGroup(float startAlpha, float endAlpha, float alphaTime)
    {
        canvasGroup = InitializeOrUpdateLerpValue(canvasGroup, startAlpha, endAlpha, alphaTime, (x) =>
        {
            GameplayUI.instance.CanvasGroup.alpha = x;
        });
    }

    private LerpValue InitializeOrUpdateLerpValue(LerpValue lerpValue, float startValue, float endValue, float duration, Action<float> updateAction)
    {
        if (lerpValue == null)
        {
            lerpValue = new LerpValue(startValue, endValue, duration, updateAction);
        }
        else
        {
            lerpValue.UpdateNewValue(startValue, endValue, duration);

            if (!lerpValue.isRuning)
            {
                lerpValue.RunAgain();
            }
        }

        return lerpValue;
    }
}
