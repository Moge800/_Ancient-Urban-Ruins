using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace AncientMarket_Libraray
{
    public class WorldObjectComp_Scavenger : WorldObjectComp
    {
        public override void PostMapGenerate()
        {
            base.PostMapGenerate();
            this.timeToSpawn = new IntRange(60000, 3 * 60000).RandomInRange;
        }
        public override void CompTick()
        {
            base.CompTick();
            if (this.ParentHasMap && this.parent.IsHashIntervalTick(this.timeToSpawn) && ((MapParent)this.parent).Map is Map map && RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f, false, null))
            {
                Faction f = Find.FactionManager.RandomRaidableEnemyFaction(false, false, false);
                Pawn t = (Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(f.RandomPawnKind(), f), cell, map, Rot4.Random, WipeMode.Vanish, false, false);
                Lord l = map.lordManager.lords.Find(l2 => l2.LordJob.GetType() == typeof(LordJob_Scavenger));
                if (l == null)
                {
                    l = LordMaker.MakeNewLord(f, new LordJob_Scavenger(), map, new List<Pawn>() { t });
                }
                else 
                {
                    l.AddPawn(t);
                }
                this.timeToSpawn = new IntRange(60000, 3 * 60000).RandomInRange;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.timeToSpawn, "timeToSpawn");
        }
        public int timeToSpawn = 0;
    }
}
