public interface ICardData
{
    public SkillData SkillData { get; }
    public bool IsSelect { get; }
    public void SetSelect(bool flag);
    public ICardData SetCard(int index);
    public ICardData RemoveCard();
}