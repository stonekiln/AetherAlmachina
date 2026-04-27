using System;
using AetherAlmachina.Skill;
using DIVFactor.Injectable;
using UnityEngine;

namespace AetherAlmachina.Card.Object
{
    /// <summary>
    /// カードのオブジェクトの親オブジェクトとなるクラス
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CardBase : MonoBehaviour, ICardData, IInjectable
    {
        CardSelector selector;
        SkillData skillData;
        public SkillData SkillData => skillData;
        public bool IsSelect => selector.isSelect;
        Action EntryEndPoint;
        public RectTransform RectTransform { get; set; }
        public Vector2 InitialSize { get; set; }

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out skillData);
            resolver.Inject(out selector);
            EntryEndPoint = resolver.EntryEndPoint;

            RectTransform = GetComponent<RectTransform>();
            InitialSize = RectTransform.rect.size;
        }

        public ICardData SetCard(int index)
        {
            transform.SetSiblingIndex(index);
            return this;
        }
        public ICardData RemoveCard()
        {
            EntryEndPoint();
            return this;
        }

        public void SetSelect(bool flag)
        {
            selector.isSelect = flag;
        }
    }
}