using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AncientMarket_Libraray
{
    public class BuildingTrader : Building, ITrader
    {
        public ModExtension_Trader Extension => this.def.GetModExtension<ModExtension_Trader>();
        public TraderKindDef TraderKind => this.Extension.tradeKind;

        public IEnumerable<Thing> Goods => this.things;

        public int RandomPriceFactorSeed => Gen.HashCombineInt(this.thingIDNumber, 1149275593);

        public string TraderName => this.Label;

        public bool CanTradeNow => true;

        public float TradePriceImprovementOffsetForPlayer => 0f;

        public TradeCurrency TradeCurrency => this.TraderKind.tradeCurrency;
        public override void SetFaction(Faction newFaction, Pawn recruiter = null)
        {
            base.SetFaction(newFaction, recruiter);
			this.factionInt = null;
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
			if (!this.init) 
			{
				this.TraderKind.stockGenerators.ForEach(or => this.things.AddRange(or.GenerateThings(this.Map.Tile)));
				this.factionInt = null;
				this.init = true;
			}
        }
        protected override void Tick()
        {
            base.Tick();
			if (this.Extension.updateGoods && this.IsHashIntervalTick(this.Extension.intervalToUpdateGoods))
			{
				this.things.Clear();
				this.TraderKind.stockGenerators.ForEach(or => this.things.AddRange(or.GenerateThings(this.Map.Tile)));
			}
        }
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
		{
			foreach (FloatMenuOption o in base.GetFloatMenuOptions(selPawn))
			{
				yield return o;
			}
			if (selPawn.CanReserveAndReach(this, PathEndMode.Touch, Danger.Deadly) && selPawn.skills != null && !selPawn.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
			{
				yield return new FloatMenuOption("AM_Trade".Translate(),() =>
                {
					selPawn.jobs.StopAll();
					Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("AM_StartTrade"), this);
					selPawn.jobs.StartJob(job);
				});
			}
			yield break;
		}
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
			if (this.Extension.leaveGoods) 
			{
				this.things.ForEach(t =>
				{
					if (t.stackCount / 10 >= 1)
					{
						t.stackCount /= 10;
						GenSpawn.Spawn(t, this.Position, this.Map);
					}
				});
				this.things.Clear();
			}
			base.Destroy(mode);
        }
        public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
		{
			IEnumerable<Thing> enumerable = from x in this.Map.listerThings.AllThings
											where 
											(x is Pawn p && p.Faction == Faction.OfPlayer) || (x.def.category == ThingCategory.Item && TradeUtility.PlayerSellableNow(x, this) && !x.IsForbidden(playerNegotiator)) && !x.Position.Fogged(x.Map)
											select x;
			foreach (Thing thing in enumerable)
			{
				yield return thing;
			}
			yield break;
        }

		public void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
		{
			Pawn pawn = toGive as Pawn;
			if (pawn != null)
			{
				pawn.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);
				Lord lord = pawn.GetLord();
				if (lord != null)
				{
					lord.Notify_PawnLost(pawn, PawnLostCondition.Undefined, null);
				}
			}
			else
			{
				IntVec3 positionHeld = this.Position;
				Map mapHeld = this.Map;
				Thing thing = toGive.SplitOff(countToGive);
				thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);
				if (GenPlace.TryPlaceThing(thing, positionHeld, mapHeld, ThingPlaceMode.Near, null, null, default(Rot4)))
				{
					Lord lord2 = this.GetLord();
					if (lord2 != null)
					{
						lord2.extraForbiddenThings.Add(thing);
						return;
					}
				}
				else
				{
					Log.Error(string.Concat(new object[]
					{
						"Could not place bought thing ",
						thing,
						" at ",
						positionHeld
					}));
					thing.Destroy(DestroyMode.Vanish);
				}
			}
			if (this.things.Contains(toGive))
			{
				this.things.Remove(toGive);
			} 
		}

        public void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
			if (this.Goods.Contains(toGive))
			{
				Log.Error("Tried to add " + toGive + " to stock (pawn's trader tracker), but it's already here.");
				return;
			}
			Thing thing = toGive.SplitOff(countToGive);
			thing.PreTraded(TradeAction.PlayerSells, playerNegotiator, this);
			Thing thing2 = TradeUtility.ThingFromStockToMergeWith(this, thing);
			if (thing2 != null)
			{
				if (!thing2.TryAbsorbStack(thing, false))
				{
					this.things.Add(thing);
					if (thing.Spawned)
					{
						thing.DeSpawn(DestroyMode.Vanish);
					}
				}
				else
				{
					thing2.stackCount += thing.stackCount;
					if (!thing.Destroyed)
					{
						thing.Destroy(DestroyMode.Vanish);
					}
				}
			}
			else 
			{
				this.things.Add(thing);
				if (thing.Spawned)
				{
					thing.DeSpawn(DestroyMode.Vanish);
				}
			}
		}
		public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref this.init,"init");
            Scribe_Collections.Look(ref this.things, "things",LookMode.Deep);
        }

		public bool init = false;
        public List<Thing> things = new List<Thing>();
    }
}
