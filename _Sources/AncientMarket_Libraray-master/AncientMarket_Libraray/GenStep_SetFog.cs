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
    public class GenStep_SetFog : GenStep
    {
        public override int SeedPart => 546516544;

        public override void Generate(Map map, GenStepParams parms)
        {
            CellIndices cellIndices = map.cellIndices;
            NativeArray<bool> f = (NativeArray<bool>)typeof(FogGrid).GetField("fogGrid", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(map.fogGrid);
            foreach (IntVec3 c in map.AllCells)
            {
                f[cellIndices.CellToIndex(c)] = true;
            }
            if (Current.ProgramState == ProgramState.Playing)
            {
                map.roofGrid.Drawer.SetDirty();
            }
        }
    }
}