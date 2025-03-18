using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace AncientMarket_Libraray
{
    public class QuestNode_GenerateCustomSite : QuestNode
	{
		protected override void RunInt()
		{
			Quest quest = QuestGen.quest;
			Slate slate = QuestGen.slate;
			Faction faction = this.faction.GetValue(slate);
			int tile = -1;
			if ((this.tile != null && !this.tile.ToString().NullOrEmpty() && this.tile.TryGetValue(slate, out tile)) || TileFinder.TryFindPassableTileWithTraversalDistance(Find.AnyPlayerHomeMap.Tile, this.distance.min, this.distance.max, out tile, (x) => this.blacklist == null || !this.blacklist.Contains(Find.World.grid[x].biome)))
			{
				if (tile == -1)
				{
					return;
				}
				Site site = this.GenerateCustomSite(this.GetParams(), tile, faction, false, null);
				quest.SpawnWorldObject(site, null, null);
				slate.Set<Site>(this.storeAs.GetValue(slate), site);
			}
			else
			{
				quest.End(QuestEndOutcome.Fail);
			}
		}
		public IEnumerable<SitePartDefWithParams> GetParams() 
		{
			if (this.siteParts.TryGetValue(QuestGen.slate,out List<SitePartDef> defs)) 
			{
				foreach (SitePartDef d in defs) 
				{
					yield return new SitePartDefWithParams(d,new SitePartParams());
				}
			}
			yield break;
		}
		protected override bool TestRunInt(Slate slate)
		{
			return true;
		}
		public Site GenerateCustomSite(IEnumerable<SitePartDefWithParams> sitePartsParams, int tile, Faction faction, bool hiddenSitePartsPossible = false, RulePack singleSitePartRules = null)
		{
			Slate slate = QuestGen.slate;
			bool flag = false;
			using (IEnumerator<SitePartDefWithParams> enumerator = sitePartsParams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.def.defaultHidden)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag || hiddenSitePartsPossible)
			{
				SitePartParams parms = SitePartDefOf.PossibleUnknownThreatMarker.Worker.GenerateDefaultParams(0f, tile, faction);
				SitePartDefWithParams val = new SitePartDefWithParams(SitePartDefOf.PossibleUnknownThreatMarker, parms);
				sitePartsParams = sitePartsParams.Concat(Gen.YieldSingle<SitePartDefWithParams>(val));
			}
			Site site = this.MakeCustomSite(sitePartsParams, tile, faction, true);
			List<Rule> list = new List<Rule>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> list2 = new List<string>();
			int num = 0;
			for (int i = 0; i < site.parts.Count; i++)
			{
				List<Rule> list3 = new List<Rule>();
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				site.parts[i].def.Worker.Notify_GeneratedByQuestGen(site.parts[i], QuestGen.slate, list3, dictionary2);
				if (!site.parts[i].hidden)
				{
					if (singleSitePartRules != null)
					{
						List<Rule> list4 = new List<Rule>();
						list4.AddRange(list3);
						list4.AddRange(singleSitePartRules.Rules);
						string text = QuestGenUtility.ResolveLocalText(list4, dictionary2, "root", false);
						list.Add(new Rule_String("sitePart" + num + "_description", text));
						if (!text.NullOrEmpty())
						{
							list2.Add(text);
						}
					}
					for (int j = 0; j < list3.Count; j++)
					{
						Rule rule = list3[j].DeepCopy();
						Rule_String rule_String = rule as Rule_String;
						if (rule_String != null && num != 0)
						{
							rule_String.keyword = string.Concat(new object[]
							{
								"sitePart",
								num,
								"_",
								rule_String.keyword
							});
						}
						list.Add(rule);
					}
					foreach (KeyValuePair<string, string> keyValuePair in dictionary2)
					{
						string text2 = keyValuePair.Key;
						if (num != 0)
						{
							text2 = string.Concat(new object[]
							{
								"sitePart",
								num,
								"_",
								text2
							});
						}
						if (!dictionary.ContainsKey(text2))
						{
							dictionary.Add(text2, keyValuePair.Value);
						}
					}
					num++;
				}
			}
			if (!list2.Any<string>())
			{
				list.Add(new Rule_String("allSitePartsDescriptions", "HiddenOrNoSitePartDescription".Translate()));
				list.Add(new Rule_String("allSitePartsDescriptionsExceptFirst", "HiddenOrNoSitePartDescription".Translate()));
			}
			else
			{
				list.Add(new Rule_String("allSitePartsDescriptions", list2.ToClauseSequence().Resolve()));
				if (list2.Count >= 2)
				{
					list.Add(new Rule_String("allSitePartsDescriptionsExceptFirst", list2.Skip(1).ToList<string>().ToClauseSequence().Resolve()));
				}
				else
				{
					list.Add(new Rule_String("allSitePartsDescriptionsExceptFirst", "HiddenOrNoSitePartDescription".Translate()));
				}
			}
			QuestGen.AddQuestDescriptionRules(list);
			QuestGen.AddQuestNameRules(list);
			QuestGen.AddQuestDescriptionConstants(dictionary);
			QuestGen.AddQuestNameConstants(dictionary);
			QuestGen.AddQuestNameRules(new List<Rule>
			{
				new Rule_String("site_label", site.Label)
			});
			return site;
		}

		public Site MakeCustomSite(IEnumerable<SitePartDefWithParams> siteParts, int tile, Faction faction, bool ifHostileThenMustRemainHostile = true)
		{
			Site site = (Site)WorldObjectMaker.MakeWorldObject(this.worldObjectDef.GetValue(QuestGen.slate));
			site.Tile = tile;
			site.SetFaction(faction);
			if (ifHostileThenMustRemainHostile && faction != null && faction.HostileTo(Faction.OfPlayer))
			{
				site.factionMustRemainHostile = true;
			}
			if (siteParts != null)
			{
				foreach (SitePartDefWithParams sitePartDefWithParams in siteParts)
				{
					site.AddPart(new SitePart(site, sitePartDefWithParams.def, sitePartDefWithParams.parms));
				}
			}
			site.desiredThreatPoints = site.ActualThreatPoints;
			return site;
		}

		public SlateRef<WorldObjectDef> worldObjectDef;
		public SlateRef<List<SitePartDef>> siteParts;
		[NoTranslate]
		public SlateRef<string> storeAs;
		public SlateRef<Faction> faction;
		public IntRange distance = new IntRange(10, 20);
		public List<BiomeDef> blacklist = new List<BiomeDef>();
		public SlateRef<int> tile;
	}
}
