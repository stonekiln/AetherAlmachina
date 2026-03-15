using AetherAlmachina.Entities.Brain;
using DIVFactor.Injectable;
using UnityEngine;

namespace AetherAlmachina.Entities.Type
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

        // Update is called once per frame
        void Update()
        {

        }


    }
}