using AncientMarket_Libraray;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AncientMarketAI_Libraray
{
    public class JobDriver_OpenLootbox : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, true, false);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Lootbox box = this.TargetThingA as Lootbox;
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch);
            Toil t = Toils_General.Wait(box.tickToOpen, TargetIndex.A);
            t.AddFinishAction(() => 
            {
                box.OpenByNPC(this.pawn);
                
                if (Rand.Chance(0.2f))
                {
                    this.pawn.mindState.duty = new PawnDuty(DutyDefOf.ExitMapBest);
                }
            });
            yield return t;
            yield break;
        }
    }
}
