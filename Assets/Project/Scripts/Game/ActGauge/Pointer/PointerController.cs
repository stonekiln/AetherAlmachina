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
        ActivatedSkillData skillData;
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
            entryEnd = resolver.EntryEndPoint;
        }
        /// <summary>
        /// 初期化処理
        /// </summary>
        void SetUp()
        {
            rectTransform = GetComponent<RectTransform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = spawnerData.Color;

            if (skillData.IsDeferrable)
            {
                remainingTime = Formula.GetTime(skillData.User.Status.Get(StatusType.Speed));
            }
            else
            {
                remainingTime = 0f;
                spriteRenderer.sortingOrder = -1;
            }
            //そのスキルの発動時間とディスプレイに表示される時間の割合とゲージの幅を掛けることで、そのスキルのポインターの初期位置を割り出す
            rectTransform.anchoredPosition = new(spawnerData.Transform.rect.xMax * remainingTime / DisplayThresholdSeconds, rectTransform.anchoredPosition.y);
        }
        public void Start()
        {
            tween = rectTransform.DOAnchorPosX(0f, remainingTime).SetEase(Ease.Linear).OnComplete(() => Trigger());
        }

        /// <summary>
        /// スキルを発動させる
        /// </summary>
        void Trigger()
        {
            Debug.Log(skillData.Name + "が発動しました。");
            DoSkillEffectsImmediately();
            skillData.User.Process.SkillEnd.OnNext(new());
            entryEnd();
        }

        //TODO:アニメーション連動を実装したら削除すること
        /// <summary>
        /// 暫定的にWhile文で効果を最後まで1フレームで実行する。後々モーションに合わせた効果の発動ができるようにすること。
        /// </summary>
        void DoSkillEffectsImmediately()
        {
            while (skillData.MoveNext()) ;
        }
    }
}