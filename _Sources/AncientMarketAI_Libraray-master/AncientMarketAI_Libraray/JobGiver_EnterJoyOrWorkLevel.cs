using AncientMarket_Libraray;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace AncientMarketAI_Libraray
{
    public class JobGiver_EnterJoyOrWorkLevel : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            GameComponent_AncientMarket gComp = GameComponent_AncientMarket.GetComp;
            if (gComp.GetSchedule(pawn) is LevelSchedule schedule && schedule.CD <= 0 && schedule.timeSchedule[GenLocalDate.HourOfDay(pawn.Map)])
            {
                if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Work && schedule.workLevel is MapEntrance work && pawn.Map != work.customMap)
                {
                    if (LevelPather.GetPathPortal(pawn.Map, work) is List<AMMapPortal> portals && portals.Any()
                        && pawn.CanReach(portals.First(), PathEndMode.Touch, Danger.Deadly))
                    {
                        if (portals.Count == 1) 
                        {
                            schedule.CD = 1200;
                            gComp.schedules_CD.Add(schedule);
                        }
                        return JobMaker.MakeJob(JobDefOf.EnterPortal, portals.First());
                    }
                }
                if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy && schedule.joyLevel is MapEntrance joy && pawn.Map != joy.customMap)
                {
                    if (LevelPather.GetPathPortal(pawn.Map, joy) is List<AMMapPortal> portals && portals.Any()
                        && pawn.CanReach(portals.First(), PathEndMode.Touch, Danger.Deadly))
                    {
                        if (portals.Count == 1)
                        {
                            schedule.CD = 1200;
                            gComp.schedules_CD.Add(schedule);
                        }
                        return JobMaker.MakeJob(JobDefOf.EnterPortal, portals.First());
                    }
                }
                if (pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Sleep && schedule.sleepLevel is MapEntrance sleep && pawn.Map != sleep.customMap)
                {
                    if (LevelPather.GetPathPortal(pawn.Map, sleep) is List<AMMapPortal> portals && portals.Any()
                        && pawn.CanReach(portals.First(), PathEndMode.Touch, Danger.Deadly))
                    {
                        if (portals.Count == 1)
                        {
                            schedule.CD = 1200;
                            gComp.schedules_CD.Add(schedule);
                        }
                        return JobMaker.MakeJob(JobDefOf.EnterPortal, portals.First());
                    }
                }
            }
            return null;
        }
    }
}
