using System;
using DIVFactor.Injectable;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace AetherAlmachina.Card.Object
{
    /// <summary>
    /// カードの外見を設定するためのクラス
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class CardDesign : MonoBehaviour, IInjectable
    {
        CardBase parent;
        public RectTransform RectTransform { get; set; }
        public Vector2 InitialPosition { get; set; }
        Image image;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out parent);

            resolver.ActivePoint.Subscribe(_ => Initialize()).AddTo(this);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Initialize()
        {
            RectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            image.sprite = parent.SkillData.Icon;
        }
    }
}