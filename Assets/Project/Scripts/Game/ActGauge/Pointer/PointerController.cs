using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill;
using DIVFactor.Injectable;
using R3;
using UnityEngine;
using DG.Tweening;
using System;

namespace AetherAlmachina.ActGauge.Pointer
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class PointerController : MonoBehaviour, IInjectable
    {
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

        void SetUp()
        {
            rectTransform = GetComponent<RectTransform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = spawnerData.Color;

            remainingTime = Formula.GetTime(skillData.User.Status.Get(StatusType.Speed));
            rectTransform.anchoredPosition = new(spawnerData.Transform.rect.xMax * remainingTime / DisplayThresholdSeconds, rectTransform.anchoredPosition.y);
        }

        void Trigger()
        {
            Debug.Log(skillData.Name + "が発動しました。");
            while (skillData.MoveNext()) ;
            skillData.User.Command.SkillEnd.OnNext(new());
            entryEnd();
        }


    }
}