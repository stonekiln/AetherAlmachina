public class CardData : ICardData
{
    SkillData skillData;
    public SkillData SkillData => skillData;
    public bool IsSelect { get; set; }

    public CardData(SkillData data)
    {
        skillData = data;
    }

    public ICardData SetCard(int index)
    {
        return this;
    }
    public ICardData RemomveCard()
    {
        return this;
    }
}