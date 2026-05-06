using UnityEngine;

namespace AetherAlmachina.ActGauge.Pointer
{
    /// <summary>
    /// 味方陣営のポインターのスポナー
    /// </summary>
    public class FriendlyPointer : PointerSpawner
    {
        protected override Color PointerColor => Color.blue;
    }
}