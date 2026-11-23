using DIVFactor.Lifecycle;
using AetherAlmachina.Skill;
using DIVFactor.Injectable;
using UnityEngine;

namespace AetherAlmachina.Card.Object
{
    /// <summary>
    /// カードのオブジェクトの親オブジェクトとなるクラス
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CardBase : MonoBehaviour, ICardData, IInitializableComponent, IReleasable
    {
        CardSelector selector;
        SkillData skillData;
        public SkillData SkillData => skillData;
        public bool IsSelect => selector.isSelect;
        LifecycleEventHub lifecycle;
        public RectTransform RectTransform { get; set; }
        public Vector2 InitialSize { get; set; }
        Vector2 initialSizeDelta;
        Vector3 initialScale;
        bool layoutCaptured;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out skillData);
            resolver.Inject(out selector);
            resolver.Inject(out lifecycle);

        }

        public void Initialize()
        {
            RectTransform = GetComponent<RectTransform>();
            if (!layoutCaptured)
            {
                InitialSize = RectTransform.rect.size;
                initialSizeDelta = RectTransform.sizeDelta;
                initialScale = transform.localScale;
                layoutCaptured = true;
            }
            ResetLayout();
        }

        public void Release() => ResetLayout();

        void ResetLayout()
        {
            if (!layoutCaptured || !RectTransform) return;
            RectTransform.sizeDelta = initialSizeDelta;
            transform.localScale = initialScale;
        }

        public ICardData SetCard(int index)
        {
            transform.SetSiblingIndex(index);
            return this;
        }
        public ICardData RemoveCard()
        {
            lifecycle.Release.OnNext(new());
            return this;
        }

        public void SetSelect(bool flag)
        {
            selector.isSelect = flag;
        }
    }
}
