using System;

public interface IPoolCallback<T>
{
    Action<T> OnCallback { get; set; }
    void OnRelease();
}
