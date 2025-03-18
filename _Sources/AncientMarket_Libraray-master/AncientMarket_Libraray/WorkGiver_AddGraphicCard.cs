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
    public class WorkGiver_AddGraphicCard : WorkGiver_Scanner
    {
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return base.HasJobOnThing(pawn, t, forced) && !t.IsForbidden(pawn) && t is GPU && pawn.Map.listerThings.GetThingsOfType<VirtualMiner>().ToList().Exists(
                m => pawn.CanReserveAndReach(m, PathEndMode.Touch, Danger.Deadly) && ((VirtualMiner)m).graphicCards.Count <= 3);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job result = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("AM_AddGPU"),t,pawn.Map.listerThings.GetThingsOfType<VirtualMiner>().ToList().Find(
                m => pawn.CanReserveAndReach(m,PathEndMode.Touch,Danger.Deadly) && ((VirtualMiner)m).graphicCards.Count <= 3));
            result.count = 1;
            return result;
        }
    }
}
