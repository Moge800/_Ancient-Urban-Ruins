using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class MapComponent_Submap : MapComponent
    {
        public MapComponent_Submap(Map map) : base(map) { }
        public static MapComponent_Submap GetComp(Map map)
        {
            return map.GetComponent<MapComponent_Submap>();
        }
        public List<MapParent_Custom> Submaps
        {
            get
            {
                if (this.submaps == null)
                {
                    this.submaps = new List<MapParent_Custom>();
                }
                this.submaps.RemoveAll(m => m == null || m.Map == null || m.Map.Disposed || m.Destroyed);
                return this.submaps;
            }
        }
        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (!this.init)
            {
                this.map.listerThings.GetThingsOfType<MapEntrance>().ToList().ForEach(m =>
         {
             if (m.customMap != null && !this.submaps.Contains(m.customMap.Parent))
             {
                 this.submaps.Add((MapParent_Custom)m.customMap.Parent);
             }
         });
                this.init = true;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref this.submaps, "submaps", LookMode.Reference);
        }

        public bool init = false;
        private List<MapParent_Custom> submaps = new List<MapParent_Custom>();
    }
}