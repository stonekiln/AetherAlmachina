using AetherAlmachina.Skill;
using DIVFactor.Event;

namespace DConfig.StageLife.Event
{
    /// <summary>
    /// スキルの発動を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Data">発動したスキル</param>
    public record SkillActivateEvent(SkillData Data) : EventObject;
}