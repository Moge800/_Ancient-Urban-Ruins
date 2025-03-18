using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class GenStep_SetTerrain : GenStep
    {
        public override int SeedPart => 12121;
        public override void Generate(Map map, GenStepParams parms)
        {
            BeachMaker.Init(map);
            TerrainDef nullDef = AMDefOf.AM_RCFloor;
            TerrainGrid terrainGrid = map.terrainGrid;
            foreach (IntVec3 c in map.AllCells)
            {
                terrainGrid.SetTerrain(c, nullDef);
            }
            BeachMaker.Cleanup();
            foreach (TerrainPatchMaker terrainPatchMaker in map.Biome.terrainPatchMakers)
            {
                terrainPatchMaker.Cleanup();
            }
            if (GenStep_SetTerrain.customMap != null)
            {
                MapGeneratingUtility.SetRoofAndTerrain(map, customMap, IntVec3.Zero);
                customMap = null;
            }
        }
        public static CustomMapDataDef customMap;
    }
}
