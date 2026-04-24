using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill;
using DIVFactor.Injectable;
using R3;
using UnityEngine;
using DG.Tweening;
using System;

namespace AetherAlmachina.ActGauge.Pointer
{
    /// <summary>
    /// ポインターの動きを制御するためのクラス
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class PointerController : MonoBehaviour, IInjectable
    {
        /// <summary>
        /// ポインターがディスプレイに表示される時間の最大値(sec)
        /// </summary>
        const float DisplayThresholdSeconds = 10f;
        readonly DelayFormula Formula = new();
        SkillData skillData;
        Tween tween;
        PointerSpawnerData spawnerData;
        RectTransform rectTransform;
        SpriteRenderer spriteRenderer;
        float remainingTime;
        Action entryEnd;

        public void Injection(InjectableResolver resolver)
        {
            resolver.Inject(out skillData);
            resolver.Inject(out spawnerData);

            resolver.ActivePoint.Subscribe(_ => SetUp()).AddTo(this);
            entryEnd = () => resolver.EntryEndPoint();
        }

        public void Start()
        {
            tween = rectTransform.DOAnchorPosX(0f, remainingTime).SetEase(Ease.Linear).OnComplete(() => Trigger());
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void SetUp()
        {
            rectTransform = GetComponent<RectTransform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = spawnerData.Color;

            remainingTime = Formula.GetTime(skillData.User.Status.Get(StatusType.Speed));
            //そのスキルの発動時間とディスプレイに表示される時間の割合とゲージの幅を掛けることで、そのスキルのポインターの初期位置を割り出す
            rectTransform.anchoredPosition = new(spawnerData.Transform.rect.xMax * remainingTime / DisplayThresholdSeconds, rectTransform.anchoredPosition.y);
        }

        /// <summary>
        /// スキルを発動させる
        /// </summary>
        void Trigger()
        {
            Debug.Log(skillData.Name + "が発動しました。");
            // TODO: 暫定的にWhile文で効果を最後まで1フレームで実行する。後々モーションに合わせた効果の発動ができるようにすること。
            while (skillData.MoveNext()) ;
            skillData.User.Command.SkillEnd.OnNext(new());
            entryEnd();
        }
    }
}