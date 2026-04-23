using UnityEngine;

namespace AetherAlmachina.ActGauge.Pointer
{
    /// <summary>
    /// 敵陣営のポインターのスポナー
    /// </summary>
    public class HostilePointer : PointerSpawner
    {
        protected override Color PointerColor => Color.red;
    }
}