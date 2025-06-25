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
    public class JobGiver_EnterSubMapByNeeding : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (AM_ModSetting.setting.enableAICrossLevel && pawn.mindState.lastJobTag == JobTag.Idle && pawn.needs?.food != null &&
                (pawn.needs.food.CurCategory >= HungerCategory.UrgentlyHungry) && 
                (!pawn.IsColonist || GameComponent_AncientMarket.GetComp.GetSchedule(pawn) is LevelSchedule schedule && schedule.timeSchedule[GenLocalDate.HourOfDay(pawn.Map)]))
            {
                List<MapPortal> available = new List<MapPortal>();
                MapComponent_Submap.GetComp(pawn.Map).Submaps.FindAll(m => m.entrance != null && pawn.CanReach(m.entrance, PathEndMode.Touch, Danger.Deadly) && m.entrance.IsAvailable(pawn)).ForEach(m => available.Add(m.entrance));
                if (pawn.Map.Parent is MapParent_Custom custom && custom.Exit != null && pawn.CanReach(custom.Exit, PathEndMode.Touch, Danger.Deadly) && custom.Exit.IsAvailable(pawn))
                {
                    available.Add(custom.Exit);
                }
                if (available.Any())
                {
                    return JobMaker.MakeJob(JobDefOf.EnterPortal, available.RandomElement());
                }

            }
            return null;
        }
    }
}