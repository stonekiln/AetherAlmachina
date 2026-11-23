using DIVFactor.Lifecycle;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill;
using DIVFactor.Injectable;
using UnityEngine;
using DG.Tweening;

namespace AetherAlmachina.ActGauge.Pointer
{
    /// <summary>
    /// ポインターの動きを制御するためのクラス
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class PointerController : MonoBehaviour, IActiveView, IUsageSubscriber
    {
        /// <summary>
        /// ポインターがディスプレイに表示される時間の最大値(sec)
        /// </summary>
        const float DisplayThresholdSeconds = 10f;
        readonly DelayFormula Formula = new();
        ActivatedSkillData skillData;
        Tween tween;
        PointerSpawnerData spawnerData;
        RectTransform rectTransform;
        SpriteRenderer spriteRenderer;
        float remainingTime;
        LifecycleEventHub lifecycle;
        UsageLifetime usage;
        bool layoutCaptured;
        int initialSortingOrder;
        Color initialColor;
        Vector2 initialPosition;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out skillData);
            resolver.Inject(out spawnerData);

            resolver.Inject(out lifecycle);
        }
        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (!layoutCaptured)
            {
                initialSortingOrder = spriteRenderer.sortingOrder;
                initialColor = spriteRenderer.color;
                initialPosition = rectTransform.anchoredPosition;
                layoutCaptured = true;
            }

            if (skillData.IsDeferrable)
            {
                remainingTime = Formula.GetTime(skillData.User.Status.Get(StatusType.Speed));
            }
            else
            {
                remainingTime = 0f;

            }
        }

        public void UpdateView()
        {
            spriteRenderer.color = spawnerData.Color;
            spriteRenderer.sortingOrder = skillData.IsDeferrable ? initialSortingOrder : -1;
            //そのスキルの発動時間とディスプレイに表示される時間の割合とゲージの幅を掛けることで、そのスキルのポインターの初期位置を割り出す
            rectTransform.anchoredPosition = new(spawnerData.Transform.rect.xMax * remainingTime / DisplayThresholdSeconds, initialPosition.y);
        }
        public void Subscribe(UsageLifetime lifetime)
        {
            usage = lifetime;
            if (skillData.User is AetherAlmachina.Entities.Entity owner)
            {
                if (!owner) throw new System.InvalidOperationException("スキルの使用者は破棄されています。");
                LifecycleEventHub currentEvents = lifecycle;
                owner.destroyCancellationToken.Register(() => currentEvents.Release.OnNext(new())).AddTo(lifetime);
            }
        }

        public void Activate()
        {
            UsageLifetime currentUsage = usage;
            ActivatedSkillData currentSkill = skillData;
            LifecycleEventHub currentEvents = lifecycle;
            tween = rectTransform.DOAnchorPosX(0f, remainingTime).SetEase(Ease.Linear);
            tween.OnComplete(() =>
            {
                if (!currentUsage.IsDisposed) Trigger(currentSkill, currentEvents, currentUsage);
            });
        }

        public void Release()
        {
            tween?.Kill();
            tween = null;
            usage = null;
            remainingTime = 0f;
            if (spriteRenderer && layoutCaptured)
            {
                spriteRenderer.sortingOrder = initialSortingOrder;
                spriteRenderer.color = initialColor;
            }
            if (rectTransform && layoutCaptured) rectTransform.anchoredPosition = initialPosition;
        }

        /// <summary>
        /// スキルを発動させる
        /// </summary>
        void Trigger(ActivatedSkillData currentSkill, LifecycleEventHub currentEvents, UsageLifetime currentUsage)
        {
            Debug.Log(currentSkill.Name + "が発動しました。");
            DoSkillEffectsImmediately(currentSkill, currentUsage);
            if (currentUsage.IsDisposed) return;
            currentSkill.User.Interaction.SkillEnd.OnNext(new());
            currentEvents.Release.OnNext(new());
        }

        //TODO:アニメーション連動を実装したら削除すること
        /// <summary>
        /// 暫定的にWhile文で効果を最後まで1フレームで実行する。後々モーションに合わせた効果の発動ができるようにすること。
        /// </summary>
        void DoSkillEffectsImmediately(ActivatedSkillData currentSkill, UsageLifetime currentUsage)
        {
            while (!currentUsage.IsDisposed && !currentSkill.MoveNext()) ;
        }
    }
}
