using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AncientMarket_Libraray
{
    public class JobDriver_Trade : JobDriver
	{
		private ITrader Trader
		{
			get
			{
				return (ITrader)base.TargetThingA;
			}
		}
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.TargetThingA, this.job, 1, -1, null, errorOnFailed, false);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch, false).FailOn(() => !this.Trader.CanTradeNow);
			Toil trade = ToilMaker.MakeToil("MakeNewToils");
			trade.initAction = delegate ()
			{
				Pawn actor = trade.actor;
				if (this.Trader.CanTradeNow)
				{
					Find.WindowStack.Add(new Dialog_Trade(actor, this.Trader, false));
				}
			};
			yield return trade;
			yield break;
		}
	}
}
