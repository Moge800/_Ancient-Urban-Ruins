using HarmonyLib;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    [HarmonyPatch(typeof(Site), "PreferredMapSize", MethodType.Getter)]
    public class Patch_QuestMapSize
    {
        [HarmonyPostfix]
        public static void postfix(Site __instance, ref IntVec3 __result)
        {
            if (__instance is CustomSite site && site.mapDef is CustomMapDataDef def)
            {
                __result.x = def.size.x;
                __result.z = def.size.z;
            }
        }
    }
}
