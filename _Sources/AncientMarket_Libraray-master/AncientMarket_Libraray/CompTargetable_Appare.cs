using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class CompProperties_Targetable_AppareWeapon : CompProperties_Targetable
    {
        public CompProperties_Targetable_AppareWeapon()
        {
            this.compClass = typeof(CompTargetable_Appare);
        }
    }
    public class CompTargetable_Appare : CompTargetable
    {
        protected override bool PlayerChoosesTarget => true;

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
            yield break;
        }

        protected override TargetingParameters GetTargetingParameters()
        {
           return new TargetingParameters()
           {
               canTargetPawns = false,
               canTargetBuildings = false,
               canTargetItems = true,
               mapObjectTargetsMustBeAutoAttackable = false,
               validator = t => t.Thing as Pawn == null
           };
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return base.ValidateTarget(target, showMessages) && target.Thing is Thing t
                && (t.def.IsApparel || t.def.IsWeapon) && t.def.useHitPoints && t.HitPoints < t.MaxHitPoints;
        }

    }
}
