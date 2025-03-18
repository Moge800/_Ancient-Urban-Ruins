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
    public class JobDriver_Repair : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job, 1, -1, null, errorOnFailed)
                && this.pawn.Reserve(this.TargetThingB, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            CompTargetEffect_Repair comp = this.TargetThingA.TryGetComp<CompTargetEffect_Repair>();
            yield return Toils_Goto.Goto(TargetIndex.A,PathEndMode.Touch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
            Toil repair = Toils_General.WaitWith(TargetIndex.B, comp.Props.repairTick,true);
            repair.AddFinishAction(() => 
            {
                this.TargetThingB.HitPoints += (int)(this.TargetThingB.MaxHitPoints * comp.Props.repairPercentage);
                this.TargetThingB.HitPoints = Math.Min(this.TargetThingB.HitPoints,this.TargetThingB.MaxHitPoints);
                this.TargetThingA.Destroy();
            });
            yield return repair;
            yield break;
        }
    }
}
