using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
	[StaticConstructorOnStartup]
	[DefOf]
	static class AMDefOf
	{
		public static JobDef AM_Repair;

        public static PawnTableDef AM_LevelSchedule;

		public static WorldObjectDef AM_CustomMap_SubMap;

        public static TerrainDef AM_RCFloor;
    }
}
