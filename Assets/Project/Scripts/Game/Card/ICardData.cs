using AetherAlmachina.Skill;

namespace AetherAlmachina.Card
{
    /// <summary>
    /// これが付与されたクラスはカードの機能を持つ
    /// </summary>
    public interface ICardData
    {
        /// <summary>
        /// カードのスキル
        /// </summary>
        public SkillData SkillData { get; }
        /// <summary>
        /// カードが選択状態
        /// </summary>
        public bool IsSelect { get; }
        /// <summary>
        /// カードの選択状態を設定する
        /// </summary>
        /// <param name="flag">設定する状態</param>
        public void SetSelect(bool flag);
        /// <summary>
        /// カードを指定された順番にセットする
        /// </summary>
        /// <param name="index">指定する順番</param>
        /// <returns>自身のカード情報</returns>
        public ICardData SetCard(int index);
        /// <summary>
        /// カードを消費する
        /// </summary>
        /// <returns></returns>
        public ICardData RemoveCard();
    }
}