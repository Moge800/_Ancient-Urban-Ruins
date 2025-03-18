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
    public class JobGier_EnterSubMap : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Job result = null;
            if (AM_ModSetting.setting.enableAICrossLevel && pawn.Faction != null && !pawn.Faction.IsPlayer)
            {
                if (!pawn.Map.mapPawns.PawnsInFaction(Faction.OfPlayer).Exists(p => (p.RaceProps.Humanlike || p.IsColonyMech) && pawn.CanReach(p, PathEndMode.Touch, Danger.Deadly))
                    && (pawn.mindState.duty is PawnDuty duty && (duty.def == DutyDefOf.AssaultColony 
                    || (duty.def == DutyDefOf.Steal && !StealAIUtility.TryFindBestItemToSteal(pawn.Position, pawn.Map, 50f, out Thing t, pawn)))))
                {
                    if (MapComponent_Submap.GetComp(pawn.Map).Submaps.Any() && MapComponent_Submap.GetComp(pawn.Map).Submaps.FindAll(e => pawn.CanReach(e.entrance, PathEndMode.Touch, Danger.Deadly) && e.entrance.IsAvailable(pawn)) is List<MapParent_Custom> entrances)
                    {
                        entrances.SortBy(e => -e.Map.wealthWatcher.HealthTotal);
                        if (entrances.Any() && entrances.First().entrance is Thing entrance)
                        {
                            result = JobMaker.MakeJob(JobDefOf.EnterPortal, entrance);
                        }
                    }
                }
            }
            return result;
        }
    }
}
