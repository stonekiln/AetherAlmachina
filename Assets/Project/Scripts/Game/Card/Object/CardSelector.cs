using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using DIVFactor.Injectable;
using DConfig.EntityLife.Event;
using Tools;

namespace AetherAlmachina.Card.Object
{
    /// <summary>
    /// カードを画面から選択するためのクラス
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CardSelector : ButtonBase, IInjectable
    {
        CardActiveEventBundle CardActive;
        CardBase parent;
        CardDesign design;
        RectTransform rectTransform;
        Vector2 initialPosition;
        readonly Vector2 ExtraSpacing = new(40f, 0);
        readonly Vector2 Offset = new(0, 20f);

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out CardActive);
            resolver.Inject(out parent);
            resolver.Inject(out design);

            resolver.ActivePoint.Subscribe(_ => Initialize()).AddTo(this);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
            initialPosition = rectTransform.anchoredPosition;
        }
        void OnEnable()
        {
            OnPointerClickAsObservable().Subscribe(eventData => MyPointerClick(eventData)).AddTo(this);
            OnPointerDownAsObservable().Subscribe(eventData => Push()).AddTo(this);
            OnPointerUpAsObservable().Subscribe(eventData => Release()).AddTo(this);
            OnPointerEnterAsObservable().Subscribe(eventData => Hover()).AddTo(this);
            OnPointerExitAsObservable().Subscribe(eventData => UnHover()).AddTo(this);
        }

        public override void SetActive()
        {

        }
        public override void SetInActive()
        {
            transform.localScale = Vector3.zero;
        }
        protected override void Hover()
        {
            isHover = true;
            parent.RectTransform.sizeDelta = parent.InitialSize + ExtraSpacing;
            design.RectTransform.anchoredPosition = design.InitialPosition + Offset;
        }
        protected override void UnHover()
        {
            if (!isSelect)
            {
                isHover = false;
                parent.RectTransform.sizeDelta = parent.InitialSize;
                design.RectTransform.anchoredPosition = design.InitialPosition;
            }
        }
        protected override void Push()
        {

        }
        protected override void Release()
        {
            if (isHover)
            {

            }
        }

        public void MyPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!isSelect)
                {
                    CardActive.Select.OnNext(new(parent, parent.transform.GetSiblingIndex()));
                    rectTransform.anchoredPosition = initialPosition + Offset;
                }
                else
                {
                    CardActive.Invoke.OnNext(new());
                }
            }
            if (eventData.button == PointerEventData.InputButton.Right && isSelect)
            {
                CardActive.Cancel.OnNext(new(parent, parent.transform.GetSiblingIndex()));
                rectTransform.anchoredPosition = initialPosition;
            }
        }
    }
}