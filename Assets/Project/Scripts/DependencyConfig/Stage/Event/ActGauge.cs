using AetherAlmachina.Skill;
using DIVFactor.Event;
using DIVFactor.Extensions;
using VContainer;
using VContainer.Unity;

namespace DConfig.StageLife.Event
{
    /// <summary>
    /// スキルの発動を宣言するイベントメッセージ
    /// </summary>
    /// <param name="Data">発動したスキル</param>
    public record SkillActivateEvent(ActivatedSkillData Data) : EventObject;

    /// <summary>
    /// 行動ゲージに関するイベントのDI登録
    /// </summary>
    public class ActGaugeInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEvent<SkillActivateEvent>();
        }
    }
}