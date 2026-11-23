using UnityEngine;

/// <summary>Playerの表示用UIの配置先。既存のPrefab参照を維持する。</summary>
namespace AetherAlmachina.Stage.UI
{
    public class PlayerUIRoot : MonoBehaviour
    {
        void Awake() => GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
