using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using DConfig.Battle.Event;
using DivFacter.Injectable;

/// <summary>
/// カードを画面に表示するためのクラス
/// </summary>
public class HandVisualizer : MonoBehaviour, IInjectable
{
    /// <summary>
    /// 手札にあるスキルのインデックスを記憶するためのクラス
    /// </summary>
    /// <param name="CallBack"></param>
    /// <param name="HandIndex"></param>
    record NumberingAction(Action CallBack, int HandIndex);
    const int HandLimit = 5;
    const int Stack = 1;
    const int Chain = 2;
    DeckDrawEventBundle DeckDraw;
    CardActivateEventBundle CardActivate;
    CardCreateEventBundle CardCreate;

    [SerializeField] HandPowerTable handPowerTable;
    [SerializeField] protected MonitoredEntity handMonitoringEntity;

    List<NumberingAction> SelectedCard;
    int Type => GetHandType();
    List<CardBase> hand;

    public void InjectDependencies(InjectableResolver resolver)
    {
        resolver.Inject(out DeckDraw);
        resolver.Inject(out CardActivate);
        resolver.Inject(out CardCreate);
    }

    void Awake()
    {
        SelectedCard = new();
    }

    void OnEnable()
    {
        DeckDraw.Response.Subscribe(response => response.DrawCard.ForEach(card => CardCreate.Request.Publish(new(card)))).AddTo(this);
        CardCreate.Response.Subscribe(response => AddHand(response.GameObject)).AddTo(this);
        CardActivate.Select.Subscribe(log => hand[log.Index].Selecter.isSelect = AddCallBacks(() => hand[log.Index].Data.Activate(), log.Index)).AddTo(this);
        CardActivate.Cancel.Subscribe(log => hand[log.Index].Selecter.isSelect = RemoveCallBack(log.Index)).AddTo(this);
        CardActivate.Invoke.Subscribe(_ => Invoke()).AddTo(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("カードをドローします");
        hand = new();
        Draw(HandLimit);
    }

    /// <summary>
    /// カードを引く
    /// </summary>
    /// <param name="count">引く枚数</param>
    void Draw(int count)
    {
        hand = hand.Where(card => card != null).ToList();
        DeckDraw.Request.Publish(new(count));
    }
    /// <summary>
    /// 手札を表示する
    /// </summary>
    /// <param name="AddCard"></param>
    void AddHand(GameObject AddCard)
    {
        CardBase SetCard(CardBase child)
        {
            child.transform.SetParent(transform, false);
            return child;
        }
        CardBase SetHand(CardBase child, int index)
        {
            child.transform.SetSiblingIndex(index);
            return child;
        }

        hand.Add(SetCard(AddCard.GetComponent<CardBase>()));
        hand = hand.OrderBy(card => card.Data.Cost).Select((card, index) => SetHand(card, index)).ToList();
    }
    /// <summary>
    /// 選択したカードを実行する
    /// </summary>
    public void Invoke()
    {
        int costSum = (int)MathF.Ceiling(SelectedCard.Aggregate(0, (previous, current) => previous + hand[current.HandIndex].Data.Cost) / SelectedCard.Count);
        if (costSum <= handMonitoringEntity.magicPoint.Value)
        {
            SetHandPower(Type, SelectedCard.Count);
            SelectedCard.ForEach(card => card.CallBack.Invoke());
            SelectedCard.ForEach(card =>
            {
                Destroy(hand[card.HandIndex].gameObject);
                hand[card.HandIndex] = null;
            });
            Draw(SelectedCard.Count);
            handMonitoringEntity.magicPoint.Value -= costSum;
            SelectedCard.Clear();
        }
        else
        {
            Debug.Log("コストが足りません");
        }
    }
    /// <summary>
    /// 選択したスキルの効果を追加する
    /// </summary>
    /// <param name="action">追加するスキル</param>
    /// <param name="index">そのスキルが手札の何番目にあるか</param>
    /// <returns>正常終了したかどうか</returns>
    bool AddCallBacks(Action action, int index)
    {
        switch (SetSelectType(index))
        {
            case 0:
                return false;
            case 1:
                SelectedCard.Add(new(action, index));
                break;
            case 2:
                SelectedCard.Insert(0, new(action, index));
                break;
        }
        return true;
    }
    /// <summary>
    /// 選択したスキルの効果を削除する
    /// </summary>
    /// <param name="index">削除するスキルがその手札の何番目にあるか</param>
    /// <returns>正常終了したかどうか</returns>
    bool RemoveCallBack(int index)
    {
        if (Type == Chain && SelectedCard.First().HandIndex != index && SelectedCard.Last().HandIndex != index)
        {
            return true;
        }
        SelectedCard = SelectedCard.Where(card => card.HandIndex != index).ToList();
        return false;
    }
    //選択されたコストの連結方法を設定し、どのように連結できるかを返す
    int SetSelectType(int index)
    {
        //まだ何も選択されていないなら
        if (SelectedCard.Count == 0)
        {
            return 1;
        }
        //先頭と同じかつStackならば1(末尾)を返す
        if (hand[SelectedCard.Last().HandIndex].Data.Cost == hand[index].Data.Cost && Type == Stack)
        {
            return 1;
        }
        if (SelectedCard.Count == 1 || Type == Chain)
        {
            //末尾よりも一つ大きいコストならば1(末尾)を返す
            if (hand[SelectedCard.Last().HandIndex].Data.Cost + 1 == hand[index].Data.Cost)
            {
                return 1;
            }
            //先頭よりも一つ小さいコストならば2(先頭)を返す
            if (hand[SelectedCard.First().HandIndex].Data.Cost - 1 == hand[index].Data.Cost)
            {
                return 2;
            }
        }
        //いずれも該当しないならば連結不可(0)を返す
        return 0;
    }
    /// <summary>
    /// 現在の役のタイプを判定する
    /// </summary>
    /// <returns>判定結果</returns>
    int GetHandType()
    {
        if (SelectedCard.Count <= 1)
        {
            return Stack;
        }
        return SelectedCard.GroupBy(card => hand[card.HandIndex].Data.Cost).Count() == 1 ? Stack : Chain;
    }

    /// <summary>
    /// 役倍率を設定する
    /// </summary>
    /// <param name="type">役の種類</param>
    /// <param name="count">成立枚数</param>
    void SetHandPower(int type, int count)
    {
        handMonitoringEntity.SetHandPowerEvent.OnNext(handPowerTable.Get(type, count));
    }
}
