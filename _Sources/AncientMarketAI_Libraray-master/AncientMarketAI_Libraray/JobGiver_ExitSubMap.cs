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
    class JobGiver_ExitSubMap : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Job result = null;
            if (AM_ModSetting .setting.enableAICrossLevel && pawn.Faction != null && !pawn.Faction.IsPlayer && pawn.Map.Parent is MapParent_Custom custom && pawn.CanReach(custom.Exit, PathEndMode.Touch, Danger.Deadly))
            {
                if (pawn.mindState.duty is PawnDuty duty
                    && (pawn.MentalStateDef == MentalStateDefOf.PanicFlee || duty.def == DutyDefOf.ExitMapRandom || duty.def == DutyDefOf.ExitMapBestAndDefendSelf || duty.def == DutyDefOf.ExitMapBest))
                {
                    result = JobMaker.MakeJob(JobDefOf.EnterPortal, custom.Exit);
                }

            }
            return result;
        }
    }
}