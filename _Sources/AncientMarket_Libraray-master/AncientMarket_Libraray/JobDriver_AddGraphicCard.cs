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
    public class JobDriver_AddGraphicCard : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.Goto(TargetIndex.A,PathEndMode.Touch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.B,PathEndMode.Touch);
            yield return new Toil() 
            {
            initAction = () => 
            {
                Thing gpu = this.TargetThingA;
                VirtualMiner miner = this.TargetThingB as VirtualMiner;
                pawn.carryTracker.innerContainer.Remove(gpu);
                miner.graphicCards.Add(gpu);
                pawn.Map.mapDrawer.MapMeshDirty(miner.Position,MapMeshFlagDefOf.Things);
            }
            };
            yield break;
        }
    }
}
