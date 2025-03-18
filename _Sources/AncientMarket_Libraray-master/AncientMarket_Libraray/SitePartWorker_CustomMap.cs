using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
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
            CustomMapDataDef def = this.def.GetModExtension<ModExtension_Map>().maps.RandomElement();
            MapGeneratingUtility.SpawnCustomMap(map, def, map.Center - new IntVec3(def.size.x / 2,0,def.size.z / 2));
        }
    }
}
