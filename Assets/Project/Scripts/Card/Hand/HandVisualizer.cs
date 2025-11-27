using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using LSES.Battle.Event;
using VContainer;

public class HandVisualizer : MonoBehaviour
{
    record NumberingAction(Action CallBack, int HandIndex);
    const int HandLimit = 5;
    const int Stack = 1;
    const int Chain = 2;

    [Inject] DeckDrawEventBundle DeckDraw;
    [Inject] CardActivateEventBundle CardActive;
    
    [SerializeField] HandPowerTable handPowerTable;
    [SerializeField] protected MonitoredEntity handMonitoringEntity;
    
    List<NumberingAction> SelectedCard;
    int Type => GetHandType();
    List<CardManager> hand;

    void Awake()
    {
        SelectedCard = new();
    }

    void OnEnable()
    {
        DeckDraw.Response.Subscribe(response => AddHand(response.DrawCard)).AddTo(this);
        CardActive.Select.Subscribe(log => hand[log.Index].Selecter.isSelect = AddCallBacks(() => hand[log.Index].Data.Activate(), log.Index)).AddTo(this);
        CardActive.Cancel.Subscribe(log => hand[log.Index].Selecter.isSelect = RemoveCallBack(log.Index)).AddTo(this);
        CardActive.Invoke.Subscribe(_ => Invoke()).AddTo(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("カードをドローします");
        hand = new();
        Draw(HandLimit);
    }

    void Draw(int count)
    {
        DeckDraw.Request.Publish(new(count));
    }

    void AddHand(List<SkillData> drawCard)
    {
        CardManager SetCard(CardManager child)
        {
            child.transform.SetParent(transform, false);
            return child;
        }
        CardManager SetHand(CardManager child, int index)
        {
            child.transform.SetSiblingIndex(index);
            return child;
        }

        hand = hand.Where(card => card != null)
                   .Concat(drawCard.Select(card => SetCard(card.CreateObject().GetComponent<CardManager>())))
                   .OrderBy(card => card.Data.Cost).Select((card, index) => SetHand(card, index)).ToList();
    }
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
    int GetHandType()
    {
        if (SelectedCard.Count <= 1)
        {
            return Stack;
        }
        return SelectedCard.GroupBy(card => hand[card.HandIndex].Data.Cost).Count() == 1 ? Stack : Chain;
    }

    void SetHandPower(int type, int count)
    {
        handMonitoringEntity.SetHandPowerEvent.OnNext(handPowerTable.Get(type, count));
    }
}
