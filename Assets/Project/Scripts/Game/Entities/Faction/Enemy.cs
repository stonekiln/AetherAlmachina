using AetherAlmachina.Entities.Brain;
using DIVFactor.Injectable;

namespace AetherAlmachina.Entities.Faction
{
    /// <summary>
    /// エネミーのMonoBehaviour
    /// </summary>
    public class Enemy : Entity
    {
        BrainBase brain;

        public override void Injection(InjectableResolver resolver)
        {
            base.Injection(resolver);
        }

        void Update()
        {

        }
    }
}