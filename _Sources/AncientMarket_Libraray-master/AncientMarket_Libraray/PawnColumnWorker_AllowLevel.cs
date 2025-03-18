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
	public class PawnColumnWorker_AllowLevel : PawnColumnWorker
	{
		public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
		{
			if (GameComponent_AncientMarket.GetComp == null)
			{
				return;
			}
			if (Widgets.ButtonText(rect,"AllowLevel".Translate(),false)) 
			{
				Find.WindowStack.Add(new Window_AllowLevel(pawn));
			}

		}
		public override int GetMinWidth(PawnTable table)
		{
			return 120;
		}
		public override int GetOptimalWidth(PawnTable table)
		{
			return 120;
		}
		public override int GetMaxWidth(PawnTable table)
		{
			return 120;
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
	}
}