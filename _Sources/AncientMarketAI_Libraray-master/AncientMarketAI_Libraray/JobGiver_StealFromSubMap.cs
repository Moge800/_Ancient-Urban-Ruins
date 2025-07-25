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
    public class JobGiver_StealFromSubMap : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Job result = null;
            if (AM_ModSetting.setting.enableAICrossLevel && pawn.Faction != null 
                && !pawn.Faction.IsPlayer && pawn.Map.Parent is MapParent_Custom custom && custom.Exit != null &&
                pawn.CanReach(custom.Exit, PathEndMode.Touch, Danger.Deadly)
                && pawn.mindState.duty is PawnDuty duty && duty.def == DutyDefOf.Steal && StealAIUtility.TryFindBestItemToSteal(pawn.Position, pawn.Map, 50f, out Thing t, pawn))
            {
                result = JobMaker.MakeJob(JobDefOf.CarryDownedPawnToPortal, custom.Exit, t);
                result.count = Mathf.Min(t.stackCount, (int)(pawn.GetStatValue(StatDefOf.CarryingCapacity, true, -1) / t.def.VolumePerUnit));
            }
            return result;
        }
    }
}