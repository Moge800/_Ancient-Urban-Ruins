using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AncientMarket_Libraray
{
    internal class PawnColumnWorker_CopyPasteLevelTimetable: PawnColumnWorker_CopyPaste
    {
        protected override bool AnythingInClipboard
        {
            get
            {
                return PawnColumnWorker_CopyPasteLevelTimetable.clipboard != null;
            }
        }
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn.timetable == null)
            {
                return;
            }
            if (pawn.IsMutant)
            {
                return;
            }
            base.DoCell(rect, pawn, table);
        }
        protected override void CopyFrom(Pawn p)
        {
            GameComponent_AncientMarket comp = GameComponent_AncientMarket.GetComp;
            LevelSchedule schedule = comp.GetSchedule(p);
            PawnColumnWorker_CopyPasteLevelTimetable.clipboard = schedule.timeSchedule;
            PawnColumnWorker_CopyPasteLevelTimetable.allowedLevels = schedule.allowedLevels;
            PawnColumnWorker_CopyPasteLevelTimetable.sleepLevel = schedule.sleepLevel;
            PawnColumnWorker_CopyPasteLevelTimetable.workLevel = schedule.workLevel;
            PawnColumnWorker_CopyPasteLevelTimetable.joyLevel = schedule.joyLevel;
        }
        protected override void PasteTo(Pawn p)
        {
            GameComponent_AncientMarket comp = GameComponent_AncientMarket.GetComp;
            LevelSchedule schedule = comp.GetSchedule(p);
            for (int i = 0; i < 24; i++)
            {
                schedule.timeSchedule[i] = PawnColumnWorker_CopyPasteLevelTimetable.clipboard[i];
            }
            schedule.allowedLevels = PawnColumnWorker_CopyPasteLevelTimetable.allowedLevels;
            schedule.sleepLevel = PawnColumnWorker_CopyPasteLevelTimetable.sleepLevel;
            schedule.workLevel = PawnColumnWorker_CopyPasteLevelTimetable.workLevel;
            schedule.joyLevel = PawnColumnWorker_CopyPasteLevelTimetable.joyLevel;
        }

        public static List<AMMapPortal> allowedLevels = new List<AMMapPortal>();
        public static AMMapPortal sleepLevel;
        public static AMMapPortal workLevel;
        public static AMMapPortal joyLevel;
        private static List<bool> clipboard;
    }
}
