using System;

public class ClickCallBacks
{
    Action callBacks;

    public void AddCallBacks(Action callBack)
    {
        callBacks += callBack;
    }
    public void Invoke()
    {
        callBacks.Invoke();
        callBacks = null;
    }
}