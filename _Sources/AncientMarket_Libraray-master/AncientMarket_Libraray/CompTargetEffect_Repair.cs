using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AncientMarket_Libraray
{
    public class CompProperties_TargetEffectRepair : CompProperties
    {
        public CompProperties_TargetEffectRepair()
        {
            this.compClass = typeof(CompTargetEffect_Repair);
        }

        public int repairTick = 120;
        public float repairPercentage = 0.5f;
    }
    public class CompTargetEffect_Repair : CompTargetEffect
    {
        public CompProperties_TargetEffectRepair Props => (CompProperties_TargetEffectRepair)this.props;
        public override void DoEffectOn(Pawn user, Thing target)
        {
            if (user.CanReserveAndReach(target, PathEndMode.Touch, Danger.Deadly))
            {
                Job j = JobMaker.MakeJob(AMDefOf.AM_Repair, this.parent, target);
                j.count = 1;
                user.jobs.StartJob(j);
            }
            else 
            {
                Messages.Message("AbilityCannotReachTarget".Translate(),MessageTypeDefOf.NegativeEvent);
            }
        }
    }
}
