#if SERVER
using Code.Libaries.Generic.Managers;
using UnityEngine;

namespace Server.Model.Entities.Human.Mythical
{
    public class Zombie : Human
    {
        protected override void Awake()
        {
            base.Awake();
            Display.ModelID = 50;
            Movement._rotationSpeed /= 3f;
            Spells.EquipSpell(ContentManager.I.Spells[3], 0);
        }

        public Player target;

        public override void Progress(float time)
        {
            base.Progress(time);
            if (!Movement.IsWalkingSomeWhere && Random.Range(0f, 100f) > 99.0f && target == null)
            {
                DoRandomWalk();
            }
            if (target != null)
            {
                if (!Movement.IsWalkingSomeWhere)
                Attack();
            }
            if (Random.Range(0f, 100f) > 99.0f)
            {
                if (target == null)
                {
                    foreach (var o in CurrentBranch.ActiveObjectsVisible)
                    {
                        if (Vector3.Distance(o.GetPosition(), GetPosition()) < 7f)
                        {
                            Player p = o as Player;
                            if (p != null)
                            {
                                target = p;
                            }
                        }
                    }
                }

            }
        }

        private void Attack()
        {
            

            if (target == null)
                return;

            float distance = Vector3.Distance(target.Movement.Position, Movement.Position);
            if (distance < 1.5f)
                Spells.FinishSpell(0);
            else if (distance < 4f)
            {
                if (Spells.CurrentCastingSpell == null)
                    Spells.StartSpell(0);
                Movement.WalkTo((Movement.Position + target.Movement.Position*2)/3f, Attack);
            }
            else
            {
                if (Spells.CurrentCastingSpell != null)
                Spells.FinishSpell(0);
                Movement.WalkTo((Movement.Position + target.Movement.Position*2) / 3f, Attack);
            }
            if (Random.Range(0f, 100f) > 99.0f)
            {
                target = null;
            }
            
        }

        private void DoRandomWalk()
        {
            Movement.WalkTo(Movement.Position + new Vector3(Random.Range(-5f, 5f), 0, (Random.Range(-5f, 5f))));
        }
    }
}
#endif
