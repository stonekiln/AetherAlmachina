using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using UnityEngine;
using UnityEngine.UI;

namespace AetherAlmachina.Card.Object
{
    /// <summary>
    /// カードの外見を設定するためのクラス
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class CardDesign : MonoBehaviour, IViewComponent, IReleasable
    {
        CardBase parent;
        public RectTransform RectTransform { get; set; }
        public Vector2 InitialPosition { get; set; }
        Image image;
        bool layoutCaptured;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out parent);
            resolver.Injected.SubscribeLifecycle(Initialize, resolver.Fail);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            RectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            if (!layoutCaptured)
            {
                InitialPosition = RectTransform.anchoredPosition;
                layoutCaptured = true;
            }
            RectTransform.anchoredPosition = InitialPosition;
        }
        public void UpdateView(ViewContext context) => image.sprite = parent.SkillData.Icon;

        public void Release()
        {
            if (image) image.sprite = null;
            if (layoutCaptured && RectTransform) RectTransform.anchoredPosition = InitialPosition;
        }
    }
}
