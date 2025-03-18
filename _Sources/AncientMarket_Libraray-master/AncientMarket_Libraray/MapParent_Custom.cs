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
    public class MapParent_Custom : PocketMapParent
    {
        public MapExit Exit
        {
            get
            {
                if (this.exit == null)
                {
                    this.exit = this.Map.listerThings.GetThingsOfType<MapExit>().First();
                }
                return this.exit;
            }
        }
        public ModExtension_Map Extension 
        {
            get 
            {
                if (this.extension == null) 
                {
                    this.extension = this.def.GetModExtension<ModExtension_Map>();
                }
                return this.extension;
            }
        }
        public virtual string MapName => this.mapDataDef?.label ?? this.def?.label;
        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            bool result = this.sourceMap == null || !this.sourceMap.Parent.Spawned || this.sourceMap.Parent.Destroyed;
            alsoRemoveWorldObject = this.Extension.isTemporary && result;
            return this.Extension.isTemporary && result;
        }
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
        {
            yield break;
        }
        public override IEnumerable<FloatMenuOption> GetTransportPodsFloatMenuOptions(IEnumerable<IThingHolder> pods, CompLaunchable representative)
        {
            yield break;
        }
        public override IEnumerable<FloatMenuOption> GetShuttleFloatMenuOptions(IEnumerable<IThingHolder> pods, Action<int, TransportPodsArrivalAction> launchAction)
        {
            yield break;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref this.mapDataDef, "mapDataDef");
            Scribe_References.Look(ref this.entrance, "CQF_MapParent_entrance");
            Scribe_References.Look(ref this.exit, "CQF_MapParent_Exit");
        }

        public ModExtension_Map extension;
        public MapEntrance entrance;
        private MapExit exit;
        public CustomMapDataDef mapDataDef;
    }
}