using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Verse;

namespace AncientMarket_Libraray
{
    public class GenStep_GenerateData : GenStep
    {
        public override int SeedPart => 546516544;

        public override void Generate(Map map, GenStepParams parms)
        {
            MapGeneratingUtility.SpawnCustomMap(map,((MapParent_Custom)map.Parent).mapDataDef,IntVec3.Zero);
        }
    }
}