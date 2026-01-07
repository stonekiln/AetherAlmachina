public class CardData : ICardData
{
    SkillData skillData;
    public SkillData SkillData => skillData;
    bool isSelect;
    public bool IsSelect => isSelect;

    public CardData(SkillData data)
    {
        skillData = data;
        isSelect = false;
    }

    public ICardData SetCard(int index)
    {
        return this;
    }
    public ICardData RemoveCard()
    {
        return this;
    }

    public void SetSelect(bool flag)
    {
        isSelect = flag;
    }
}