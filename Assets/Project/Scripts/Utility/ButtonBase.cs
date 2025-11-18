using System;
using R3;
using R3.Triggers;

public abstract class ButtonBase : ObservableEventTrigger
{
    public Action onClickCallback;
    public ReactiveProperty<bool> isHover = new()
    {
        Value = false
    };
    public ReactiveProperty<bool> isSelect = new()
    {
        Value = false
    };

    public abstract void SetActive();
    public abstract void SetInActive();
    protected abstract void Hover();
    protected abstract void UnHover();
    protected abstract void Push();
    protected abstract void Release();
}