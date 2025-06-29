using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class SitePartWorker_CustomMap : SitePartWorker
    {
        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);
            if (map.Parent is CustomSite site && site.mapDef != null)
            {
                CustomMapDataDef def = site.mapDef;
                MapGeneratingUtility.SpawnCustomMap(map, def, map.Center - new IntVec3(def.size.x / 2, 0, def.size.z / 2));
                FogMap(map);
                if (MapGenerator.PlayerStartSpotValid) 
                {
                    map.fogGrid.FloodUnfogAdjacent(MapGenerator.PlayerStartSpot);
                }
            }
            else
            {
                Log.Error("Null map def:" + map.Parent.ToString());
            }


        }
        public static void FogMap(Map map)
        {
            map.fogGrid.Refog(CellRect.WholeMap(map));
            if (Current.ProgramState == ProgramState.Playing)
            {
                map.roofGrid.Drawer.SetDirty();
            }
        }

    }
}
