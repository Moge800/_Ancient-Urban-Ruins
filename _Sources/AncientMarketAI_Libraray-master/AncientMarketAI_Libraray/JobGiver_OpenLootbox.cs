using AncientMarket_Libraray;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AncientMarketAI_Libraray
{
    public class JobGiver_OpenLootbox : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Faction?.def != FactionDefOf.AncientsHostile && pawn.RaceProps.Humanlike && pawn.Map.listerThings.GetThingsOfType<Lootbox>()?.ToList().Find(l => l.CanOpen && pawn.CanReserveAndReach(l,PathEndMode.Touch,Danger.Deadly)) is Thing box) 
            {
                return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("AM_OpenByNPC"),box);
            }
            return null;
        }
    }
}
