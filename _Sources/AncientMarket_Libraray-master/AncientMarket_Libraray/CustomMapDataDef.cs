using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace AncientMarket_Libraray
{
    public class CustomMapDataDef : Def
    {
        public CustomMapDataDef() { }
        public CustomMapDataDef Origin => this.origin == null ? this : this.origin;
        public CellRect GetRect(IntVec3 pos)
        {
            List<IntVec3> cells = this.GetAllPosition();
            int? minX = null;
            int? maxX = null;
            int? minZ = null;
            int? maxZ = null;
            cells.ToList().ForEach((c) =>
            {
                maxX = maxX != null && c.x < maxX ? maxX : c.x;
                minX = minX != null && c.x > minX ? minX : c.x;
                minZ = minZ != null && c.z > minZ ? minZ : c.z;
                maxZ = maxZ != null && c.z < maxZ ? maxZ : c.z;
            });

            CellRect result = CellRect.FromLimits(pos.x + minX.Value, pos.z + minZ.Value, pos.x + maxX.Value, pos.z + maxZ.Value);
            return result;
        }
        public List<IntVec3> GetAllPosition()
        {
            List<IntVec3> result = new List<IntVec3>();
            Action<IntVec3> add = p =>
            {
                if (!result.Contains(p))
                {
                    result.Add(p);
                }
            };
            this.thingDatas.ForEach(d => add(d.position));
            this.routes.Values.ToList().ForEach(p => p.ForEach(p2 => add(p2)));
            this.terrains.Values.ToList().ForEach(p => p.ForEach(p2 => add(p2)));
            this.roofs.Values.ToList().ForEach(p => p.ForEach(p2 => add(p2)));
            return result;
        }
        public CustomMapDataDef Copy(string name)
        {
            CustomMapDataDef result = new CustomMapDataDef();
            result.defName = this.defName + name;
            result.description = this.description;
            result.size = this.size;
            result.isPart = this.isPart;
            result.fogged = this.fogged;
            result.tags = this.tags.ListFullCopy();
            result.rot = this.rot;
            result.faction = this.faction;
            this.thingDatas.ForEach(d => result.thingDatas.Add(d.Copy()));
            result.terrains = new Dictionary<string, List<IntVec3>>();
            this.terrains.ToList().ForEach(t => result.terrains.Add(t.Key, t.Value));
            result.roofs = new Dictionary<RoofDef, List<IntVec3>>();
            this.roofs.ToList().ForEach(t => result.roofs.Add(t.Key, t.Value));
            result.routes = new Dictionary<string, List<IntVec3>>();
            this.routes.ToList().ForEach(t => result.routes.Add(t.Key, t.Value));
            result.origin = this;
            return result;
        }


        public bool fogged = false;
        public IntVec3 size;
        public bool isPart = false;
        public float commonality = 0.8f;
        public int generationLimit = 0;
        public string faction = null;
        public Dictionary<string, List<IntVec3>> routes = new Dictionary<string, List<IntVec3>>();
        public Dictionary<RoofDef, List<IntVec3>> roofs = new Dictionary<RoofDef, List<IntVec3>>();
        public Dictionary<string, List<IntVec3>> terrains = new Dictionary<string, List<IntVec3>>();
        public List<ThingData> thingDatas = new List<ThingData>();
        public List<string> tags = new List<string>();
        public List<IntVec3> disgenerate = new List<IntVec3>();
        public List<IntVec3> disdestroy = new List<IntVec3>();
        public List<PawnKindDefCount> pawns = new List<PawnKindDefCount>();
        public Rot4 rot = Rot4.Invalid;
        public CustomMapDataDef origin;
        public Dictionary<Rot4, CustomMapDataDef> extraDataByDirection = new Dictionary<Rot4, CustomMapDataDef>();
        public Dictionary<IntVec3, CustomMapDataDef> extraDataByOrigin = new Dictionary<IntVec3, CustomMapDataDef>();
    }

    public class ThingData : IExposable
    {
        public ThingData() { }
        public ThingData(Thing thing, IntVec3 pos)
        {
            this.def = thing.def;
            this.rotation = thing.Rotation;
            this.position = pos;
            this.count = thing.stackCount;
            this.stuff = thing.Stuff;
            this.style = thing.StyleDef;
            this.faction = thing.Faction?.def;
            if (thing.TryGetQuality(out QualityCategory q))
            {
                this.quality = q;
            }
            if (thing is Plant plant)
            {
                this.growth = plant.Growth;
            }
            if (thing.def.useHitPoints)
            {
                this.hitPoint = thing.HitPoints;
            }
            if (thing.TryGetComp<CompPowerBattery>() is CompPowerBattery compB)
            {
                this.storedEnergy = compB.StoredEnergy;
            }
            if (thing.TryGetComp<CompRefuelable>() is CompRefuelable compR)
            {
                this.storedEnergy = compR.Fuel;
            }
        }
        public Thing Spawn(Map map, IntVec3 pos, Func<ThingDef, bool, ThingDef> getDef, ThingDef forcedStuff = null, Rot4? forcedRot = null)
        {
            ThingDef def = getDef(this.def, false);
            if (def == null)
            {
                Log.Error("Spawn thing data error:" + this.ToString());
                return null;
            }
            Thing thing = ThingMaker.MakeThing(def, def.MadeFromStuff ? forcedStuff ?? getDef(this.stuff, true) : null);
            thing.stackCount = this.count;
            thing.StyleDef = this.style;
            thing.Rotation = this.rotation;
            thing.stackCount = this.count;
            if (thing.TryGetComp<CompQuality>() is CompQuality compQ)
            {
                compQ.SetQuality(this.quality, null);
            }
            if (thing.TryGetComp<CompPowerBattery>() is CompPowerBattery compB)
            {
                compB.AddEnergy(this.storedEnergy);
            }
            if (thing.TryGetComp<CompRefuelable>() is CompRefuelable compR)
            {
                compR.Refuel(this.storedEnergy);
            }
            if (thing.def.useHitPoints)
            {
                thing.HitPoints = (int)(((float)this.hitPoint / (float)this.def.GetStatValueAbstract(StatDefOf.MaxHitPoints, this.stuff ?? GenStuff.DefaultStuffFor(this.def)) * thing.MaxHitPoints));
            }
            if (thing is Plant plant)
            {
                plant.Growth = this.growth;
            }
            if (this.faction != null && Find.FactionManager.FirstFactionOfDef(this.faction) is Faction faction)
            {
                thing.SetFaction(faction);
            }
            if (thing.TryGetComp<CompColorable>() is CompColorable color)
            {
                this.color = color.Color;
            }
            if (thing is Building b)
            {
                b.ChangePaint(this.colorDef);
            }
            return GenSpawn.Spawn(thing, pos, map, forcedRot ?? this.rotation);
        }
        public ThingData Copy()
        {
            ThingData result = new ThingData();
            result.def = this.def;
            result.stuff = this.stuff;
            result.style = this.style;
            result.faction = this.faction;
            result.rotation = this.rotation;
            result.position = this.position;
            result.count = this.count;
            result.growth = this.growth;
            result.quality = this.quality;
            result.hitPoint = this.hitPoint;
            result.storedEnergy = this.storedEnergy;
            result.colorDef = this.colorDef;
            result.allPositions = this.allPositions.ListFullCopy();
            return result;
        }
        public void ExposeData()
        {
            Scribe_Defs.Look(ref this.def, "def");
            Scribe_Defs.Look(ref this.style, "style");
            Scribe_Defs.Look(ref this.stuff, "stuff");
            Scribe_Defs.Look(ref this.faction, "faction");
            Scribe_Values.Look(ref this.hitPoint, "QE_ThingData_hitPoint");
            Scribe_Values.Look(ref this.growth, "QE_ThingData_growth");
            Scribe_Values.Look(ref this.rotation, "QE_ThingData_rotation");
            Scribe_Values.Look(ref this.count, "QE_ThingData_count");
            Scribe_Values.Look(ref this.storedEnergy, "QE_ThingData_storedEnergy");
            Scribe_Values.Look(ref this.quality, "quality");
            Scribe_Collections.Look(ref this.allPositions, "positions", LookMode.Value);
        }

        public bool Equals_Def(ThingData data)
        {
            return data.def == this.def && data.stuff == this.stuff && data.style == this.style && data.faction == this.faction && data.rotation == this.rotation && data.count == this.count
               && data.hitPoint == this.hitPoint && this.growth == data.growth && this.storedEnergy == data.storedEnergy;
        }

        public ThingDef def = null;
        public ThingDef stuff = null;
        public ThingStyleDef style = null;
        public FactionDef faction = null;
        public Rot4 rotation = Rot4.North;
        public IntVec3 position = IntVec3.Zero;
        public Color color = Color.white;
        public ColorDef colorDef;
        public List<IntVec3> allPositions = new List<IntVec3>();
        public QualityCategory quality = QualityCategory.Normal;
        public int count = 1;
        public float growth = 0f;
        public int hitPoint = 1;
        public float storedEnergy;
    }

    public static class MapGeneratingUtility
    {

        public static void SpawnCustomMap(Map map, CustomMapDataDef def, IntVec3? centerP = null)
        {
            try
            {
                List<IntVec3> disgenerate2 = new List<IntVec3>();
                CustomMapDataDef origin = def;
                IntVec3 center = centerP == null ? map.Center - new IntVec3(def.size.x / 2, 0, def.size.z / 2) : centerP.Value;
                MapGeneratingUtility.Pretreat(map, def, center);
                MapGeneratingUtility.SetRoofAndTerrain(map, def, center);
                MapGeneratingUtility.SpawnThings(map, def, center);
                MapGeneratingUtility.SpawnPawns(map, def, center);
            }
            catch (Exception e)
            {
                Log.Error("Generate Custom map error:" + def?.defName + "," + e.ToString());
            }
        }
        private static void Pretreat(Map map, CustomMapDataDef def, IntVec3 center)
        {
            CellRect rect = def.GetRect(center);
            List<IntVec3> poss = rect.Cells.ToList();
            List<Thing> things = new List<Thing>();
            poss.ForEach(x =>
            {
                if (x.InBounds(map) && !def.disdestroy.Contains(x - center))
                {
                    map.roofGrid.SetRoof(x, null);
                    things.AddRange(x.GetThingList(map).ListFullCopy());
                }
            });
            while (things.Count != 0)
            {
                Thing thing = things[0];
                if (thing.def.destroyable && !thing.Destroyed && !(thing is MapExit || thing is Pawn || thing is Skyfaller))
                {
                    thing.Destroy();
                }
                if (things.Contains(thing))
                {
                    things.Remove(thing);
                }
            }
        }
        public static void SpawnPawns(Map map, CustomMapDataDef def, IntVec3 center)
        {
            def.pawns.ForEach(p =>
            {
                for (int i = 0; i < p.count; i++)
                {
                    if (map != null && p.kindDef != null && CellFinderLoose.TryFindRandomNotEdgeCellWith(5, i2 => i2.Standable(map), map, out IntVec3 pos))
                    {
                        Faction f = Find.FactionManager.FirstFactionOfDef(p.kindDef.defaultFactionType);

                        Pawn t = (Pawn)GenSpawn.Spawn(PawnGenerator.GeneratePawn(p.kindDef, f), pos, map);
                        Lord l = map.lordManager.lords.Find(l2 => l2.LordJob?.GetType() == typeof(LordJob_Scavenger) && l2.faction?.def == p.kindDef.defaultFactionType);
                        if (l == null)
                        {
                            l = LordMaker.MakeNewLord(f, new LordJob_Scavenger(), map, new List<Pawn>() { t });
                        }
                        else
                        {
                            l.AddPawn(t);
                        }
                    }
                }
            });
        }
        public static void SpawnThings(Map map, CustomMapDataDef def, IntVec3 center)
        {
            foreach (ThingData content in def.thingDatas)
            {
                List<IntVec3> poss = new List<IntVec3>();
                if (content.allPositions.Any())
                {
                    poss.AddRange(content.allPositions);
                }
                else
                {
                    poss.Add(content.position);
                }
                Faction faction = Find.FactionManager.FirstFactionOfDef(content.faction);
                poss.ForEach(p =>
                {
                    IntVec3 intVec3 = center + p;
                    if (intVec3.InBounds(map))
                    {
                        Thing t = content.Spawn(map, intVec3, (d, s) =>
                        {
                            return d;
                        }
       );
                        if (t != null && t.def.CanHaveFaction && t.Faction == null && faction != null)
                        {
                            t.SetFaction(faction);
                        }
                    }
                });
            }
        }
        public static void SetRoofAndTerrain(Map map, CustomMapDataDef def, IntVec3 center)
        {
            foreach (KeyValuePair<string, List<IntVec3>> content in def.terrains)
            {
                TerrainDef terrain = TerrainDef.Named(content.Key);
                content.Value.ForEach(x =>
                {
                    if ((x + center).InBounds(map))
                    {
                        map.terrainGrid.SetTerrain(x + center, terrain);
                    }
                });
            }
            foreach (KeyValuePair<RoofDef, List<IntVec3>> content in def.roofs)
            {
                content.Value.ForEach(x =>
                {
                    if ((x + center).InBounds(map))
                    {
                        map.roofGrid.SetRoof(x + center, content.Key);
                    }
                });
            }
        }
    }
}