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
    public class Designator_Open : Designator_Cells
	{
		public Designator_Open()
		{
            this.defaultLabel = "DesignatorOpen".Translate();
            this.defaultDesc = "DesignatorOpenDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Open", true);
            this.useMouseIcon = true;
		}
        public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Areas;
        public override bool DragDrawMeasurements
        {
            get
            {
                return true;
            }
        }
        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            return loc.GetFirstThing<Building_Casket>(Find.CurrentMap) != null;
        }
        public override void DesignateThing(Thing t)
        {
            if (t is Building_Casket && !t.Map.designationManager.HasMapDesignationOn(t)) 
            {
                t.Map.designationManager.AddDesignation(new Verse.Designation(t,DesignationDefOf.Open));
            }
        }
        public override void DesignateSingleCell(IntVec3 c)
        {
            if (c.InBounds(Find.CurrentMap) && c.GetThingList(Find.CurrentMap).Find(t0 => t0 is Building_Casket) is Building_Casket t && !t.Map.designationManager.HasMapDesignationOn(t))
            {
                t.Map.designationManager.AddDesignation(new Verse.Designation(t, DesignationDefOf.Open));
            }
        }
    }
}
