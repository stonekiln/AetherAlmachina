using System;
using System.Collections.Generic;
using System.Linq;
using EventBus.Card;
using R3;
using UnityEngine;

public abstract class HandExecutor : MonoBehaviour
{
    class NumberingAction
    {
        public int HandIndex { get; private set; }
        public Action CallBack { get; private set; }
        public NumberingAction(Action action, int index)
        {
            CallBack = action;
            HandIndex = index;
        }
    }
    [SerializeField] CardSelectEvent cardSelectEvent;
    [SerializeField] protected MonitoredEntity handMonitoringEntity;
    const int Stack = 1;
    const int Chain = 2;
    List<NumberingAction> SelectedCard;
    int Type => GetHandType();
    [NonSerialized] public List<SkillData> deck;
    protected List<CardManager> hand;
    protected abstract void Draw(int count);
    protected abstract void SetHandPower(int type, int count);

    void Awake()
    {
        cardSelectEvent.Add.Subscribe(index => hand[index].Selecter.isSelect = AddCallBacks(() => hand[index].Data.Activate(), index)).AddTo(this);
        cardSelectEvent.Remove.Subscribe(index => hand[index].Selecter.isSelect = RemoveCallBack(index)).AddTo(this);
        cardSelectEvent.Invoke.Subscribe(_ => Invoke()).AddTo(this);
        SelectedCard = new();
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
}