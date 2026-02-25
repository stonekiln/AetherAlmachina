public class CardData : ICardData
{
    public SkillData SkillData { get; private set; }
    public bool IsSelect { get; private set; }

    public CardData(SkillData data)
    {
        SkillData = data;
        IsSelect = false;
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
        IsSelect = flag;
    }
}