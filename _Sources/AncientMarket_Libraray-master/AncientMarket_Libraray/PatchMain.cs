using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
	[StaticConstructorOnStartup]
	public static class PatchMain
	{
		static PatchMain()
		{
			Harmony harmony = new Harmony("AM_Patch");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}