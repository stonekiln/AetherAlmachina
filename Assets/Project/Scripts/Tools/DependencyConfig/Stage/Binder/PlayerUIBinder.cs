using DIVFactor.Binder;
using UnityEngine;

public class PlayerUIBinder : ObjectBinder<UIChildMark>
{
    public void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}