using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace AncientMarket_Libraray
{
	[HarmonyPatch(typeof(TendUtility), "DoTend")]
	public class Patch_Tend
	{
		[HarmonyPrefix]
		public static bool prefix(Pawn doctor, Pawn patient, Medicine medicine)
		{
			if (medicine != null && medicine.TryGetComp<CompUseableCount>() is CompUseableCount comp)
			{
				if (!patient.health.HasHediffsNeedingTend(false))
				{
					return false;
				}
				if (medicine != null && medicine.Destroyed)
				{
					Log.Warning("Tried to use destroyed medicine.");
					medicine = null;
				}
				float quality = TendUtility.CalculateBaseTendQuality(doctor, patient, (medicine != null) ? medicine.def : null);
				List<Hediff> tmpHediffsToTend = (List<Hediff>)typeof(TendUtility).GetField("tmpHediffsToTend", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				TendUtility.GetOptimalHediffsToTendWithSingleTreatment(patient, medicine != null, tmpHediffsToTend, null);
				float maxQuality = (medicine != null) ? medicine.def.GetStatValueAbstract(StatDefOf.MedicalQualityMax, null) : 0.7f;
				for (int i = 0; i < tmpHediffsToTend.Count; i++)
				{
					tmpHediffsToTend[i].Tended(quality, maxQuality, i);
				}
				if (doctor != null && doctor.Faction == Faction.OfPlayer && patient.Faction != doctor.Faction && !patient.IsPrisoner && patient.Faction != null)
				{
					patient.mindState.timesGuestTendedToByPlayer++;
				}
				if (doctor != null && doctor.RaceProps.Humanlike && patient.RaceProps.Animal && patient.RaceProps.playerCanChangeMaster && RelationsUtility.TryDevelopBondRelation(doctor, patient, 0.004f) && doctor.Faction != null && doctor.Faction != patient.Faction)
				{
					InteractionWorker_RecruitAttempt.DoRecruit(doctor, patient, false);
				}
				patient.records.Increment(RecordDefOf.TimesTendedTo);
				if (doctor != null)
				{
					doctor.records.Increment(RecordDefOf.TimesTendedOther);
				}
				if (doctor == patient && !doctor.Dead)
				{
					doctor.mindState.Notify_SelfTended();
				}
				if (medicine != null)
				{
					if (comp.count > 1)
					{
						comp.count--;
					}
					else if (medicine.stackCount > 1)
					{
						medicine.stackCount--;
						comp.count = comp.Props.useableCount;
					}
					else
					if (!medicine.Destroyed)
					{
						medicine.Destroy(DestroyMode.Vanish);
					}
				}
				if (ModsConfig.IdeologyActive && ((doctor != null) ? doctor.Ideo : null) != null)
				{
					Precept_Role role = doctor.Ideo.GetRole(doctor);
					if (((role != null) ? role.def.roleEffects : null) != null)
					{
						foreach (RoleEffect roleEffect in role.def.roleEffects)
						{
							roleEffect.Notify_Tended(doctor, patient);
						}
					}
				}
				if (doctor != null && doctor.Faction == Faction.OfPlayer && doctor != patient)
				{
					QuestUtility.SendQuestTargetSignals(patient.questTags, "PlayerTended", patient.Named("SUBJECT"));
				}
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(RecipeWorker), "ConsumeIngredient")]
	public class Patch_Recipe
	{
		[HarmonyPrefix]
		public static bool prefix(Thing ingredient, RecipeDef recipe, Map map)
		{

			if (ingredient != null && ingredient.TryGetComp<CompUseableCount>() is CompUseableCount comp)
			{
				if (comp.Count > 1)
				{
					comp.count--;
					return false;
				}
				else
				{
					comp.count = comp.Props.useableCount;
				}
			}
			return true;
		}
	}
}