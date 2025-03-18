using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AncientMarket_Libraray
{
    public class Tab_LevelTransmit : ITab
    {
        public Tab_LevelTransmit()
        {
            this.size = new Vector2(400f, 300f);
            this.labelKey = "Tab_LevelTransmit";
            this.tutorTag = "LevelTransmit";
        }
        public Building_Transmit transmit
        {
            get
            {
                return (this.SelThing as Building_Transmit);
            }
        }
        public Building_Receive receive
        {
            get
            {
                return (this.SelThing as Building_Receive);
            }
        }
        protected override void FillTab()
        {     
            float y = 5f;
            float x = 5f;
            if (this.receive != null)
            {
                this.receive.name = Tab_LevelPower.DrawLabelAndText_Line(y, "ReceiverName".Translate(), ref this.receive.name, x, 100f);
            }
            if (this.transmit != null) 
            {
                if (Widgets.ButtonText(new Rect(x, y, 150f, 25f), "SelectLinkedReceiver".Translate(this.transmit.receive?.name), false))
                {
                    Map map = this.SelThing.Map;
                    List<Map> targetMaps = new List<Map>();
                    List<Thing> things = new List<Thing>();
                    if (map != null)
                    {
                        if (map.Parent is MapParent_Custom custom)
                        {
                            targetMaps.Add(custom.sourceMap);
                        }
                        if (map.GetComponent<MapComponent_Submap>() is MapComponent_Submap comp)
                        {
                            comp.Submaps.ForEach(m => targetMaps.Add(m.Map));
                        }
                    }
                    targetMaps.ForEach(m => things.AddRange(m.listerBuildings.allBuildingsColonist.FindAll(b => b is Building_Receive)));
                    if (things.Any())
                    {
                        Tab_LevelPower.DrawFloatMenu(things, t =>
                        {
                            this.transmit.receive = (Building_Receive)t;
                        }, t => (t as Building_Receive).name);
                    }
                }
            }
        }


        public CompPowerPlantLevel comp;
    }
}
