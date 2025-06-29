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
    [HarmonyPatch(typeof(HediffStatsUtility), "SpecialDisplayStats")]
    public class Patch_Stat
	{
        [HarmonyPostfix]
        public static void postfix(ref IEnumerable<StatDrawEntry> __result, HediffStage stage, Hediff instance)
        {
            if (stage != null && instance!=null) 
            {
                List<StatDrawEntry> result = __result.ToList();
                if (instance.TryGetComp<HediffComp_Regeneration>() is HediffComp_Regeneration comp && comp.Props.regenerations.TryGetValue(instance.CurStageIndex,out float v) && stage?.showRegenerationStat == true)
                {
                    result.Add(new StatDrawEntry(StatCategoryDefOf.CapacityEffects, "Stat_Hediff_Regeneration_Name".Translate(), "Stat_Hediff_Regeneration_Stat".Translate(string.Format("{0:0}", v)), "Stat_Hediff_Regeneration_Desc".Translate(), 4025, null, null, false, false));
                }
                __result = result;
            }
        }
    }
}
