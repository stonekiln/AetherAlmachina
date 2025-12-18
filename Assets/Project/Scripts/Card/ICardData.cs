public interface ICardData
{
    public SkillData SkillData { get; }
    public bool IsSelect { get; set; }
    public ICardData SetCard(int index);
    public ICardData RemomveCard();
}