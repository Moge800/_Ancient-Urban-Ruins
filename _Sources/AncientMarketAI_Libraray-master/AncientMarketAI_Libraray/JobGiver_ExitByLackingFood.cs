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
    public class JobGiver_ExitByLackingFood : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.needs?.food?.CurCategory >= HungerCategory.UrgentlyHungry)
            {
                if (pawn.Map.Parent is MapParent_Custom custom && custom.Exit != null && pawn.CanReach(custom.Exit, PathEndMode.Touch, Danger.Deadly))
                {
                    return JobMaker.MakeJob(JobDefOf.EnterPortal, custom.Exit);
                }

            }
            return null;
        }
    }
}