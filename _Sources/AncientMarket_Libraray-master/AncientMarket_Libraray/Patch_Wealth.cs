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
    [HarmonyPatch(typeof(WealthWatcher), "WealthTotal",MethodType.Getter)]
    public class Patch_Wealth
    {
        [HarmonyPostfix]
        public static void postfix(ref float __result, WealthWatcher __instance, Map ___map)
        {
            if (___map != null && MapComponent_Submap.GetComp(___map) is MapComponent_Submap comp) 
            {
                float total = 0f;
                comp.Submaps.ForEach(c => total += c.Map.wealthWatcher.WealthTotal);
                __result += total;
            }
        }
    }
}
