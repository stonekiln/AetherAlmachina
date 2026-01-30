using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using DConfig.EntityLife.Event;
using DIVFactor.Injectable;

public class HandController : MonoBehaviour, IInjectable
{
    const int HandLimit = 5;
    const int Stack = 1;
    const int Chain = 2;
    [SerializeField] HandPowerTable handPowerTable;
    DeckDrawEvent DeckDraw;
    CardActivateEventBundle CardActivate;
    Entity owner;
    List<int> selectedIndex;
    int Type => GetHandType();
    public List<ICardData> Hand { get; private set; }

    public virtual void Injection(InjectableResolver resolver)
    {
        resolver.Inject(out DeckDraw);
        resolver.Inject(out CardActivate);
        owner = resolver.GetComponent<Player>();
        selectedIndex = new();

        DeckDraw.Response(response => Hand = AddHand(response.DrawCard)).AddTo(this);
        CardActivate.Select.Subscribe(log => log.Data.SetSelect(Select(log.Index))).AddTo(this);
        CardActivate.Cancel.Subscribe(log => log.Data.SetSelect(!SelectCancel(log.Index))).AddTo(this);
        CardActivate.Invoke.Subscribe(_ => Invoke()).AddTo(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("カードをドローします");
        Hand = new();
        Draw(HandLimit);
    }

    /// <summary>
    /// カードを引く
    /// </summary>
    /// <param name="count">引く枚数</param>
    void Draw(int count)
    {
        DeckDraw.Call(new(count));
    }
    /// <summary>
    /// カードを手札に追加する
    /// </summary>
    /// <param name="skills">追加するスキル</param>
    protected virtual List<ICardData> AddHand(List<SkillData> skills)
    {
        return Hand.Concat(skills.Select(skill => new CardData(skill))).OrderBy(card => card.SkillData.Cost).ToList();
    }
    /// <summary>
    /// カードを消費する
    /// </summary>
    protected virtual List<ICardData> RemoveHand()
    {
        return Hand.Where(card => !card.IsSelect).ToList();
    }
    /// <summary>
    /// 選択したカードを実行する
    /// </summary>
    public void Invoke()
    {
        int costSum = (int)MathF.Ceiling(selectedIndex.Aggregate(0, (previous, current) => previous + Hand[current].SkillData.Cost) / (float)selectedIndex.Count());
        if (costSum <= owner.Status.magicPoint)
        {
            owner.SetHandPower(handPowerTable.Get(Type, selectedIndex.Count()));
            owner.Status.magicPoint -= costSum;
            selectedIndex.ForEach(cardIndex => Hand[cardIndex].SkillData.Activate());
            Hand = RemoveHand();
            Draw(selectedIndex.Count());
            selectedIndex = new();
        }
        else
        {
            Debug.Log("コストが足りません");
        }
    }
    /// <summary>
    /// カードを選択状態にする
    /// </summary>
    /// <param name="action">追加するスキル</param>
    /// <param name="index">そのスキルが手札の何番目にあるか</param>
    /// <returns>正常終了したかどうか</returns>
    bool Select(int index)
    {
        switch (SetSelectType(index))
        {
            case 0:
                return false;
            case 1:
                selectedIndex.Add(index);
                break;
            case 2:
                selectedIndex.Insert(0, index);
                break;
        }
        return true;
    }
    /// <summary>
    /// カードを選択解除する
    /// </summary>
    /// <param name="index">削除するスキルがその手札の何番目にあるか</param>
    /// <returns>正常終了したかどうか</returns>
    bool SelectCancel(int index)
    {
        if (Type == Chain && selectedIndex.First() != index && selectedIndex.Last() != index)
        {
            return false;
        }
        return selectedIndex.Remove(index);
    }
    //選択されたカードが、どのように連結できるかを返す
    int SetSelectType(int index)
    {
        //まだ何も選択されていないなら
        if (selectedIndex.Count == 0)
        {
            return 1;
        }
        //末尾と同じかつStackならば1(末尾)を返す
        if (Hand[selectedIndex.Last()].SkillData.Cost == Hand[index].SkillData.Cost && Type == Stack)
        {
            return 1;
        }
        if (selectedIndex.Count == 1 || Type == Chain)
        {
            //末尾よりも一つ大きいコストならば1(末尾)を返す
            if (Hand[selectedIndex.Last()].SkillData.Cost + 1 == Hand[index].SkillData.Cost)
            {
                return 1;
            }
            //先頭よりも一つ小さいコストならば2(先頭)を返す
            if (Hand[selectedIndex.First()].SkillData.Cost - 1 == Hand[index].SkillData.Cost)
            {
                return 2;
            }
        }
        //いずれも該当しないならば連結不可(0)を返す
        return 0;
    }
    /// <summary>
    /// 現在の役の種類を判定する
    /// </summary>
    /// <returns>判定結果</returns>
    int GetHandType()
    {
        if (selectedIndex.Count <= 1)
        {
            return Stack;
        }
        return selectedIndex.GroupBy(cardIndex => Hand[cardIndex].SkillData.Cost).Count() == 1 ? Stack : Chain;
    }
}