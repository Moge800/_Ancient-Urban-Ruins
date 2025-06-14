using AncientMarket_Libraray;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AncientMarketAI_Libraray
{
	[StaticConstructorOnStartup]
	public static class PatchMain
	{
		static PatchMain()
		{
			Harmony harmony = new Harmony("AM_Patch");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}

	[HarmonyPatch(typeof(JobGiver_ExitMap), "TryGiveJob")]
	public class Patch_Exit
	{
		[HarmonyPostfix]
		public static void postfix(Pawn pawn, ref Job __result)
		{
			if (__result == null && AM_ModSetting.setting.enableAICrossLevel 
				&& !pawn.Downed && !pawn.Crawling && (pawn.MentalStateDef == MentalStateDefOf.PanicFlee 
				|| (pawn.Faction !=null && !pawn.Faction.IsPlayer && pawn.GetLord() == null && pawn.mindState.duty == null) 
				|| (pawn.mindState.duty is PawnDuty duty && (duty.def == DutyDefOf.ExitMapRandom || duty.def == DutyDefOf.ExitMapBestAndDefendSelf || duty.def == DutyDefOf.ExitMapBest)))
				&& pawn.Map.Parent is MapParent_Custom custom && custom.Exit.IsAvailable(pawn) && pawn.CanReach(custom.Exit, PathEndMode.Touch, Danger.Deadly))
			{
				__result = JobMaker.MakeJob(JobDefOf.EnterPortal, custom.Exit);
			}
		}
	}

	[HarmonyPatch(typeof(JobGiver_GetRest), "TryGiveJob")]
	public class Patch_Rest
	{
		[HarmonyPostfix]
		public static void postfix(Pawn pawn, ref Job __result)
		{
			if (__result != null && AM_ModSetting.setting.enableAICrossLevel && pawn.RaceProps.Humanlike
				&& !pawn.Downed && !pawn.Crawling && __result.targetA.Thing == null && GameComponent_AncientMarket.GetComp.GetSchedule(pawn) is LevelSchedule schedule && schedule.timeSchedule[GenLocalDate.HourOfDay(pawn.Map)])
			{
                if (schedule.sleepLevel is AMMapPortal sleep && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Sleep)
                {
                    if (LevelPather.GetPathPortal(pawn.Map, sleep) is List<AMMapPortal> portals && portals.Any()
                        && pawn.CanReach(portals.First(), PathEndMode.Touch, Danger.Deadly))
                    {
                        __result = JobMaker.MakeJob(JobDefOf.EnterPortal, portals.First());
						return;
                    }
                }
                List<AMMapPortal> available = new List<AMMapPortal>();
				MapComponent_Submap.GetComp(pawn.Map).Submaps.FindAll(m => m.entrance != null && pawn.CanReach(m.entrance, PathEndMode.Touch, Danger.Deadly) && m.entrance.IsAvailable(pawn)).ForEach(m => available.Add(m.entrance));
				if (pawn.Map.Parent is MapParent_Custom custom && custom.Exit != null && custom.Exit.IsAvailable(pawn) && pawn.CanReach(custom.Exit, PathEndMode.Touch, Danger.Deadly))
				{
					available.Add(custom.Exit);
				}
				if (available.Any())
				{
					__result = JobMaker.MakeJob(JobDefOf.EnterPortal, available.RandomElement());
				}

			}
		}
	}
}