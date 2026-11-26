#if UNITY_EDITOR
using AetherAlmachina.ActGauge.Pointer;
using AetherAlmachina.Card.Object;
using AetherAlmachina.Entities;
using AetherAlmachina.Entities.Parameter;
using AetherAlmachina.Skill;
using AetherAlmachina.Skill.Effect;
using AetherAlmachina.Skill.Effect.Selectors;
using DConfig.EntityLife.Event;
using DG.Tweening;
using DIVFactor.Extensions;
using DIVFactor.Injectable;
using DIVFactor.Lifecycle;
using DIVFactor.Pooling;
using NUnit.Framework;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
using VContainer;

namespace AetherAlmachina.Editor.DIVFactor
{
    [Category("UnityEditor")]
    public class PoolingComponentUnityTests
    {
        readonly List<UnityEngine.Object> created = new();

        [UnityTest]
        public IEnumerator CardReuseResetsSelectionHoverLayoutAndSubscriptions()
        {
            yield return new EnterPlayMode();
            GameObject model = Make("card", true);
            model.SetActive(false);
            RectTransform rect = model.GetComponent<RectTransform>();
            rect.sizeDelta = new(120, 180);
            CardBase card = model.AddComponent<CardBase>();
            CardSelector selector = model.AddComponent<CardSelector>();
            GameObject imageObject = Make("card image", true);
            imageObject.transform.SetParent(model.transform, false);
            Image image = imageObject.AddComponent<Image>();
            CardDesign design = imageObject.AddComponent<CardDesign>();
            SkillData first = MakeSkill("first", false, null), second = MakeSkill("second", false, null);
            LifetimeArguments arguments = LifetimeArguments.Create(first);
            VContainer.ContainerBuilder builder = new();
            arguments.Register(builder);
            builder.RegisterEventHub<LifecycleEventHub>();
            builder.RegisterEventHub<CardActiveEventHub>();
            builder.RegisterInstance(card); builder.RegisterInstance(selector); builder.RegisterInstance(design);
            using VContainer.IObjectResolver container = builder.Build();
            using LifecycleRunner runner = new(new object[] { card, selector, design }, container.Resolve<LifecycleEventHub>(),
                () => model.SetActive(true), () => model.SetActive(false), () => UnityEngine.Object.Destroy(model),
                returnToPool: () => { }, clearArguments: arguments.Clear);
            int clicks = 0;
            using IDisposable observer = container.Resolve<CardActiveEventHub>().Select.Subscribe(_ => clicks++);
            runner.Run(new InjectableResolver(container));
            Vector2 initialSize = rect.sizeDelta;
            selector.isSelect = selector.isHover = true;
            rect.sizeDelta = new(999, 999);
            design.RectTransform.anchoredPosition = new(22, 44);
            card.RemoveCard();
            Assert.That(card.SkillData, Is.Null);
            Assert.That(selector.isSelect || selector.isHover, Is.False);
            Assert.That(rect.sizeDelta, Is.EqualTo(initialSize));
            Assert.That(image.sprite, Is.Null);
            arguments.Replace(LifetimeArguments.Create(second));
            runner.Run(new InjectableResolver(container));
            Assert.That(card.SkillData, Is.SameAs(second));
            Assert.That(image.sprite, Is.SameAs(second.Icon));
            selector.isHover = true;
            ExecuteEvents.Execute(model, new PointerEventData(null) { button = PointerEventData.InputButton.Left }, ExecuteEvents.pointerClickHandler);
            Assert.That(clicks, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator PointerReuseKillsOldTweenAndRestoresSortingForTheNextSkill()
        {
            yield return new EnterPlayMode();
            VContainer.ContainerBuilder ownerBuilder = new();
            ownerBuilder.RegisterEventHub<LockOnEventHub>();
            ownerBuilder.RegisterEventHub<InteractionEventHub>();
            using VContainer.IObjectResolver ownerContainer = ownerBuilder.Build();
            TestEntity entity = Make("skill owner").AddComponent<TestEntity>();
            StatusAsset status = ScriptableObject.CreateInstance<StatusAsset>();
            created.Add(status);
            foreach (StatusType statusType in Enum.GetValues(typeof(StatusType))) status.BaseStatus[statusType] = 1;
            status.BaseStatus[StatusType.Speed] = 100;
            Set(entity, nameof(Entity.LockOn), ownerContainer.Resolve<LockOnEventHub>(), typeof(Entity));
            Set(entity, nameof(Entity.Interaction), ownerContainer.Resolve<InteractionEventHub>(), typeof(Entity));
            Set(entity, nameof(Entity.Status), new StatusParameter(status, new ResourceStatusParameter()), typeof(Entity));
            ActivatedSkillData first = new(MakeSkill("first", false, entity), 1);
            ActivatedSkillData second = new(MakeSkill("second", true, entity), 1);
            GameObject model = Make("pointer", true);
            model.SetActive(false);
            SpriteRenderer renderer = model.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 7;
            PointerController controller = model.AddComponent<PointerController>();
            RectTransform parent = Make("pointer parent", true).GetComponent<RectTransform>();
            LifetimeArguments arguments = LifetimeArguments.Create(first, new PointerSpawnerData(parent, Color.red));
            VContainer.ContainerBuilder builder = new();
            arguments.Register(builder);
            builder.RegisterEventHub<LifecycleEventHub>();
            using VContainer.IObjectResolver container = builder.Build();
            using LifecycleRunner runner = new(new object[] { controller }, container.Resolve<LifecycleEventHub>(),
                () => model.SetActive(true), () => model.SetActive(false), () => UnityEngine.Object.Destroy(model),
                returnToPool: () => { }, clearArguments: arguments.Clear);
            runner.Run(new InjectableResolver(container));
            Tween oldTween = Get<Tween>(controller, "tween");
            LifecycleEventHub oldEvents = Get<LifecycleEventHub>(controller, "lifecycle");
            Assert.That(renderer.sortingOrder, Is.EqualTo(-1));
            oldEvents.Release.OnNext(new());
            Assert.That(oldTween.IsActive(), Is.False);
            Assert.That(Get<ActivatedSkillData>(controller, "skillData"), Is.Null);
            arguments.Replace(LifetimeArguments.Create(second, new PointerSpawnerData(parent, Color.blue)));
            runner.Run(new InjectableResolver(container));
            Assert.That(renderer.sortingOrder, Is.EqualTo(7));
            Assert.That(renderer.color, Is.EqualTo(Color.blue));
            oldEvents.Release.OnNext(new());
            Assert.That(runner.State, Is.EqualTo(LifecycleState.Active));
            Get<Tween>(controller, "tween").Complete();
            Assert.That(runner.State, Is.EqualTo(LifecycleState.Released));
            arguments.Replace(LifetimeArguments.Create(new ActivatedSkillData(MakeSkill("owner destroyed", true, entity), 1), new PointerSpawnerData(parent, Color.red)));
            runner.Run(new InjectableResolver(container));
            UnityEngine.Object.Destroy(entity.gameObject);
            yield return null;
            Assert.That(runner.State, Is.EqualTo(LifecycleState.Released));
        }

        SkillData MakeSkill(string name, bool deferred, Entity owner)
        {
            SkillAsset asset = ScriptableObject.CreateInstance<SkillAsset>();
            created.Add(asset);
            Texture2D texture = new(2, 2);
            created.Add(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.zero);
            created.Add(sprite);
            LockOnParameter target = new();
            Set(target, nameof(LockOnParameter.Selector), deferred ? (Selector)new EnemySelector() : new SelfSelector());
            Set(asset, nameof(SkillAsset.SkillName), name);
            Set(asset, nameof(SkillAsset.Icon), sprite);
            Set(asset, nameof(SkillAsset.InitialLockOn), target);
            Set(asset, nameof(SkillAsset.EffectQueue), new List<EffectData>());
            return new SkillData(asset, owner);
        }

        static void Set(object target, string property, object value, Type declaring = null)
            => (declaring ?? target.GetType()).GetProperty(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(target, value);
        static T Get<T>(object target, string field)
            => (T)target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        GameObject Make(string name, bool rectTransform = false)
        {
            GameObject instance = rectTransform ? new GameObject(name, typeof(RectTransform)) : new GameObject(name);
            created.Add(instance);
            return instance;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int index = created.Count - 1; index >= 0; index--) if (created[index]) UnityEngine.Object.Destroy(created[index]);
            created.Clear();
            yield return null;
            yield return new ExitPlayMode();
        }

        public class TestEntity : Entity { }
    }
}
#endif
