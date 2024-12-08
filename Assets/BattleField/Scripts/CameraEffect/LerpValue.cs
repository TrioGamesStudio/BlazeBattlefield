using System;
using UnityEngine;

public class LerpValue
{
    public float targetValue;
    public float startValue;
    public float currentValue;

    public Action<float> updateLerpAction;
    public Action OnCompleteCallback;
    public float duration;

    private float elapsedTime;
    public bool canLerp = true;
    public bool isRuning = false;
    public LerpValue(float _startValue, float _targetValue, float _duration, Action<float> _updateLerpAction)
    {
        startValue = _startValue;
        targetValue = _targetValue;
        currentValue = _startValue;
        duration = _duration;
        updateLerpAction = _updateLerpAction;

        RunAgain();
    }
    public void UpdateNewValue(float _startValue, float _targetValue, float _duration)
    {
        startValue = _startValue;
        targetValue = _targetValue;
        currentValue = _startValue;
        duration = _duration;
        elapsedTime = 0;
    }

    public void RunAgain()
    {
        elapsedTime = 0;
        LerpManager.Instance.AddToLerp(this);
        isRuning = true;
    }

    public void Update(float deltaTime)
    {
        if (canLerp == false) return;

        elapsedTime += deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        currentValue = Mathf.Lerp(startValue, targetValue, t);
        updateLerpAction?.Invoke(currentValue);

        if (t >= 1.0f)
        {
            LerpManager.Instance.RemoveToLerp(this);
            OnCompleteCallback?.Invoke();
            isRuning = false;
        }
    }

}
