using System;
using System.Collections.Generic;
using System.Linq;

public class ClickCallBacks
{
    const int Stack = 1;
    const int Chain = 2;
    readonly Action<int, int> setHandPower;
    Action callBacks;
    LinkedList<int> selected;
    public int type;

    public ClickCallBacks(Action<int, int> action)
    {
        setHandPower = action;
        Initialize();
    }

    public void Initialize()
    {
        callBacks = null;
        selected = new();
        type = Stack;
    }
    public bool AddCallBacks(Action action, int cardCost)
    {
        if (selected.Count == 0)
        {
            selected.AddFirst(cardCost);
            callBacks += action;
            return true;
        }
        switch (SetSelectType(cardCost))
        {
            case 0:
                return false;
            case 1:
                selected.AddLast(cardCost);
                callBacks += action;
                break;
            case 2:
                selected.AddFirst(cardCost);
                callBacks = callBacks.GetInvocationList().Aggregate(action, (previous, current) => previous + (Action)current);
                break;
        }
        return true;
    }
    public void Invoke()
    {
        setHandPower.Invoke(type, selected.Count == 0 ? 1 : selected.Count);
        callBacks.Invoke();
        Initialize();
    }
    //2番目に選択されたコストの連結方法を設定し、どのように連結できるかを返す
    int SetSelectType(int value)
    {
        //先頭よりも一つ小さい数ならば2(先頭)を返す
        if (selected.First.Value == (value - 1))
        {
            type = Chain;
            return 2;
        }
        //末尾よりも一つ大きいならば1(末尾)を返す
        else if (selected.Last.Value == (value + 1))
        {
            type = Chain;
            return 1;
        }
        //末尾と同じ かつ (そのひとつ前がnull もしくは 同じ)ならば1(末尾)を返す
        else if (selected.Last.Value == value && (selected.Last.Previous?.Value ?? value) == value)
        {
            type = Stack;
            return 1;
        }
        //いずれも該当しないならば連結不可(0)を返す
        else
        {
            return 0;
        }
    }
}