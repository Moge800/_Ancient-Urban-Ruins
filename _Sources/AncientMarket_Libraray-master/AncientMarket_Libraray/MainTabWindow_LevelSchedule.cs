using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AncientMarket_Libraray
{
    public class MainButtonWorker_LevelSchedule : MainButtonWorker_ToggleTab
    {
        public override bool Visible => base.Visible && AM_ModSetting.setting.enableAICrossLevel;
    }
    public class MainTabWindow_LevelSchedule : MainTabWindow_PawnTable
	{
		protected override PawnTableDef PawnTableDef
		{
			get
			{
				return AMDefOf.AM_LevelSchedule;
			}
		}

		protected override IEnumerable<Pawn> Pawns
		{
			get
			{
				return from pawn in base.Pawns
					   where !pawn.DevelopmentalStage.Baby() && pawn.Spawned && pawn.IsColonist
					   select pawn;
			}
		}
		protected override float ExtraTopSpace
		{
			get
			{
				return 40f;
			}
		}
		public override void DoWindowContents(Rect fillRect)
		{
			base.DoWindowContents(fillRect);
			Rect rect = new Rect(0f, 0f, 80f,35f);
			this.DrawTimeAssignmentSelectorFor(rect,true);
			rect.x += rect.width;
			this.DrawTimeAssignmentSelectorFor(rect, false);
		}

		private void DrawTimeAssignmentSelectorFor(Rect rect, bool allow)
		{
			rect = rect.ContractedBy(2f);
			GUI.DrawTexture(rect, allow ? BaseContent.WhiteTex : BaseContent.GreyTex);
			Widgets.Label(rect, allow ? "Allow".Translate().Colorize(Color.black) : "Unallow".Translate().Colorize(Color.black));
			if (Widgets.ButtonInvisible(rect, true))
			{
				MainTabWindow_LevelSchedule.allow = allow;
				SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			if (MainTabWindow_LevelSchedule.allow == allow)
			{
				Widgets.DrawBox(rect, 2, null);
				return;
			}
		}

		public static bool allow = true;
	}
}
