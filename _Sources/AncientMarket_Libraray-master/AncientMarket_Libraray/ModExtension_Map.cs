using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class ModExtension_Map : DefModExtension
    {
        public bool isTemporary = true;
        public List<CustomMapDataDef> maps = new List<CustomMapDataDef>();
    }

    public class ModExtension_Lootbox : DefModExtension
    {
        public SoundDef sound;
        public GraphicData openedGraphicdata;
        public List<ThingSetMakerDef> loots = new List<ThingSetMakerDef>();
    }
}