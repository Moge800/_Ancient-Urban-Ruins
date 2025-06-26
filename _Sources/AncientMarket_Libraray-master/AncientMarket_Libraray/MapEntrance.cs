using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Noise;

namespace AncientMarket_Libraray
{
    public class MapEntrance : AMMapPortal, IRenameable
    {
        
        public override string Label
        {
            get
            {
                if (this.customName == null) 
                {
                    this.customName = base.Label;
                }
                return this.customName;
            }
        }
        public string RenamableLabel { get => this.customName; set => this.customName = value; }
        public MapExit Exit
        {
            get
            {
                if (this.exit == null && this.customMap != null)
                {
                    if (this.customMap.listerThings.GetThingsOfType<MapExit>().Any())
                    {
                        this.exit = (PocketMapExit)this.customMap.listerThings.GetThingsOfType<MapExit>().First();
                        MapExit exitAM = exit as MapExit;
                        exitAM.entrance = this;
                    }
                    else 
                    {
                        Log.Error("AM Error:No exit in sub map"); 
                    }
                }
                return this.exit as MapExit;
            }
        }

        public string BaseLabel => base.Label;

        public string InspectLabel => this.Label;

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            yield return new Command_Action()
            {
                defaultLabel = "RenamePortal".Translate(),
                icon = TexButton.Rename,
                action = () =>
                {
                    Find.WindowStack.Add(new Dialog_RenameSubMap(this));
                }
            };
            yield break;
        }
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.customMap != null) 
            {
                this.customMap?.Parent.Destroy();
                Current.Game.DeinitAndRemoveMap(this.customMap, true);
            }
            base.Destroy(mode);
        }
        protected override void Tick()
        {
            base.Tick();
            if (this.IsHashIntervalTick(5) && this.Exit != null && this.Position.GetFirstPawn(this.Map) is Pawn p && p.Drafted)
            {
                IntVec3 pos = GetDestinationLocation();
                if (pos.IsValid)
                {
                    p.DeSpawn();
                    GenSpawn.Spawn(p, pos, this.exit.Map);
                    this.OnEntered(p);
                }
            }
        }
        public override void OnEntered(Pawn pawn)
        {
            base.OnEntered(pawn);
            if ((pawn.GetLord() is Lord l && l.LordJob is LordJob_AssaultColony) || pawn.mindState.duty?.def == DutyDefOf.AssaultColony)  
            {
                this.PawnAndLords.SetOrAdd(pawn, pawn.GetLord());
            }
            if (this.Exit.CD.Find(c => c.pawn == pawn) is CD cd)
            {
                cd.cd = 1200;
                return;
            }
            this.Exit.CD.Add(new CD() { pawn = pawn, cd = 1200 });
        }
        public void GenerateCustomMap(Map map, CustomMapDataDef def)
        {
            if (this.customMap != null || this.generatedMap)
            {
                return;
            }
            this.generatedMap = true;
            MapGenerator.PlayerStartSpot = this.Position;
            MapParent_Custom custom = (MapParent_Custom)WorldObjectMaker.MakeWorldObject(AMDefOf.AM_CustomMap_SubMap);
            custom.mapDataDef = def;
            custom.SetFaction(Find.FactionManager.OfPlayer);
            custom.entrance = this;
            custom.Tile = map.Tile;
            MapComponent_Submap.GetComp(map).Submaps.Add(custom);
            string seed = Find.World.info.seedString;
            Find.World.info.seedString = Find.TickManager.TicksGame.ToString();
            LongEventHandler.SetCurrentEventText("GenerateSubMap".Translate());
            DeepProfiler.Start("Generate map");
            custom.sourceMap = map;
            PocketMapUtility.currentlyGeneratingPortal = this;
            this.customMap = MapGenerator.GenerateMap(def.size, custom, DefDatabase<MapGeneratorDef>.GetNamed("AM_CustomMap_Editor_Generator"), null, null, true);
            map.mapPawns.FreeColonists.ForEach(p => GameComponent_AncientMarket.GetComp.GetSchedule(p).AllowedLevels.Add(this));
            Find.World.pocketMaps.Add(custom);
            Find.World.info.seedString = seed;
            DeepProfiler.End();
        }

        public override IntVec3 GetDestinationLocation()
        {
            if (this.customMap == null)
            {
                List<CustomMapDataDef> maps = this.def.GetModExtension<ModExtension_Map>().maps;
                if (!maps.Any()) 
                {
                    Log.Error("AM Error:Null map in def");
                    return IntVec3.Invalid;
                }
                this.GenerateCustomMap(this.Map, maps.RandomElement());
            }
            return this.Exit.Position + (-1 * (this.Exit.Rotation.FacingCell * (this.def.HasModExtension<ModExtension_Portal>() ? this.def.GetModExtension<ModExtension_Portal>().distance : 1))); ;
        }

        public override Map GetOtherMap()
        {
            if (this.customMap == null) 
            {
                this.GenerateCustomMap(this.Map, this.def.GetModExtension<ModExtension_Map>().maps.RandomElement());
            }
            return this.customMap;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.init, "init");
            Scribe_Values.Look(ref this.customName, "customName");
            Scribe_Values.Look(ref this.generatedMap, "generatedMap");
            Scribe_References.Look(ref this.customMap, "customMap");
        }

        public string customName;
        public bool init = false;
        public bool generatedMap = false;
        public Map customMap;
    }

    public class ModExtension_Portal : DefModExtension 
    {
        public int distance = 1;
    }
}
