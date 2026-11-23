using DIVFactor.Lifecycle;
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
    public class CardSelector : ButtonBase, IReactiveComponent, IReleasable
    {
        CardActiveEventHub CardActive;
        CardBase parent;
        CardDesign design;
        RectTransform rectTransform;
        Vector2 initialPosition;
        Vector3 initialScale;
        bool layoutCaptured;
        readonly Vector2 ExtraSpacing = new(40f, 0);
        readonly Vector2 Offset = new(0, 20f);

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out CardActive);
            resolver.Inject(out parent);
            resolver.Inject(out design);

        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
            if (!layoutCaptured)
            {
                initialPosition = rectTransform.anchoredPosition;
                initialScale = transform.localScale;
                layoutCaptured = true;
            }
            ResetInteraction();
        }

        // ButtonBase.Releaseはポインターを離した処理なので、使用終了とは分ける。
        void IReleasable.Release() => ResetInteraction();

        void ResetInteraction()
        {
            isHover = false;
            isSelect = false;
            if (!layoutCaptured || !rectTransform) return;
            rectTransform.anchoredPosition = initialPosition;
            transform.localScale = initialScale;
        }
        public void Subscribe(UsageLifetime lifetime)
        {
            OnPointerClickAsObservable().Subscribe(eventData => MyPointerClick(eventData)).AddTo(lifetime);
            OnPointerDownAsObservable().Subscribe(eventData => Push()).AddTo(lifetime);
            OnPointerUpAsObservable().Subscribe(eventData => Release()).AddTo(lifetime);
            OnPointerEnterAsObservable().Subscribe(eventData => Hover()).AddTo(lifetime);
            OnPointerExitAsObservable().Subscribe(eventData => UnHover()).AddTo(lifetime);
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
