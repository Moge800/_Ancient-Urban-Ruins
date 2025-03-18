using LudeonTK;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public static class AMUtility
	{
        public static List<MapParent_Custom> GetSubMaps(Map map)
        {
            if (map == null) 
            {
                return new List<MapParent_Custom>();
            }
            List<MapParent_Custom> result = new List<MapParent_Custom>();
            if (map.Parent is MapParent_Custom custom)
            {
                result.Add(custom);
            }
            if (map.GetComponent<MapComponent_Submap>() is MapComponent_Submap comp)
            {
                comp.Submaps.ForEach(m =>
                {
                    result.AddRange(AMUtility.GetSubMaps(m.Map));
                });
            }
            return result;
        }
        public static Map GetRootMap(Map map)
        {
            if (map?.Parent is PocketMapParent custom)
            {
                return AMUtility.GetRootMap(custom.sourceMap);
            }

            return map;
        }
        [DebugAction("AncientMarket", null, false, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void OpenAllBox()
		{
			Find.CurrentMap.listerThings.GetThingsOfType<Lootbox>().ToList().ForEach(r =>
			{
				if (r.CanOpen)
				{
					r.Open(); 
				}
			});
		}
	}
}
