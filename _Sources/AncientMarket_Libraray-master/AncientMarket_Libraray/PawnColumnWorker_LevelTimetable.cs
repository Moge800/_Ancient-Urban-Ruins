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
	public class PawnColumnWorker_LevelTimetable : PawnColumnWorker
	{
		public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
		{
			if (GameComponent_AncientMarket.GetComp == null)
			{
				return;
			}
			float num = rect.x;
			float num2 = rect.width / 24f;
			for (int i = 0; i < 24; i++)
			{
				Rect rect2 = new Rect(num, rect.y, num2, rect.height);
				this.DoTimeAssignment(rect2, pawn, i);
				num += num2;
			}
			GUI.color = Color.white;
		}
		public override void DoHeader(Rect rect, PawnTable table)
		{
			float num = rect.x;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.LowerCenter;
			float num2 = rect.width / 24f;
			for (int i = 0; i < 24; i++)
			{
				Widgets.Label(new Rect(num, rect.y, num2, rect.height + 3f), i.ToString());
				num += num2;
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		public override int GetMinWidth(PawnTable table)
		{
			return Mathf.Max(base.GetMinWidth(table), 360);
		}
		public override int GetOptimalWidth(PawnTable table)
		{
			return Mathf.Clamp(504, this.GetMinWidth(table), this.GetMaxWidth(table));
		}
		public override int GetMaxWidth(PawnTable table)
		{
			return Mathf.Min(base.GetMaxWidth(table), 600);
		}
		public override int GetMinHeaderHeight(PawnTable table)
		{
			return Mathf.Max(base.GetMinHeaderHeight(table), 15);
		}
		public override int Compare(Pawn a, Pawn b)
		{
			return this.GetValueToCompare(a).CompareTo(this.GetValueToCompare(b));
		}
		private int GetValueToCompare(Pawn pawn)
		{
			GameComponent_AncientMarket comp = GameComponent_AncientMarket.GetComp;
			return comp.GetSchedule(pawn).timeSchedule.FirstIndexOf(x => x);
		}
		private void DoTimeAssignment(Rect rect, Pawn p, int hour)
		{
			GameComponent_AncientMarket comp = GameComponent_AncientMarket.GetComp;
			if (comp.GetSchedule(p) is LevelSchedule s) 
			{
				rect = rect.ContractedBy(1f);
				bool mouseButton = Input.GetMouseButton(0);
				bool allow = s.timeSchedule[hour];
				GUI.DrawTexture(rect, allow ? BaseContent.WhiteTex : BaseContent.GreyTex);
				if (!mouseButton)
				{
					MouseoverSounds.DoRegion(rect);
				}
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawBox(rect, 2, null);
					if (mouseButton)
					{
						SoundDefOf.Designate_DragStandard_Changed_NoCam.PlayOneShotOnCamera(null);
						comp.GetSchedule(p).timeSchedule[hour] = MainTabWindow_LevelSchedule.allow;
					}
				}
			}
		}
	}
}