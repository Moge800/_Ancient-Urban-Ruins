using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AncientMarket_Libraray
{
	[StaticConstructorOnStartup]
	public class CustomSite : Site
    {
        public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}

			Command_Action command_Action = new Command_Action();
			command_Action.defaultLabel = "AbandonQuestMap".Translate();
			command_Action.defaultDesc = "AbandonQuestMapDesc".Translate();
			command_Action.icon = CustomSite.AbandonCommandTex;
			command_Action.groupKey = 111;
			command_Action.action = delegate ()
			{
				if (this.Map != null)
				{
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmAbandonQuestMap".Translate(), () =>
					{
						this.Map?.GetComponent<MapComponent_Submap>()?.Submaps.ForEach(m =>
						{
							if (!m.Destroyed)
							{
								m.Destroy();
							}
						}
						);
						this.Destroy();
					}
				));
				}
			};
			command_Action.Order = 3000f;
			yield return command_Action;
			yield break;
		}
        public override bool ShouldRemoveMapNow(out bool alsoRemoveWorldObject)
        {
            alsoRemoveWorldObject = false;
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Defs.Look(ref this.mapDef, "mapDef");
        }

        public CustomMapDataDef mapDef;
		private static readonly Texture2D AbandonCommandTex = ContentFinder<Texture2D>.Get("UI/Commands/AbandonHome", true);
	}
}
