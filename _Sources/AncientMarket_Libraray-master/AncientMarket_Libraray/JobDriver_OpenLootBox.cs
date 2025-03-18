using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;

namespace AncientMarket_Libraray
{
    public class JobDriver_OpenLootBox : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Lootbox loot = (Lootbox)this.TargetThingA;
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch);
            Toil toil = Toils_General.WaitWith(TargetIndex.A, loot.tickToOpen, true);
            toil.AddPreTickAction(() => this.pawn.rotationTracker.FaceTarget(loot.Position));
            yield return toil;
            yield return new Toil() { initAction = () => loot.Open(), defaultCompleteMode = ToilCompleteMode.Delay };
            yield break;
        }
    }
}