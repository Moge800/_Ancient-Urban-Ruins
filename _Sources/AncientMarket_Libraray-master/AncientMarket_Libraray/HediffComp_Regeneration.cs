using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AncientMarket_Libraray
{
	public class HediffCompProperties_Regeneration : HediffCompProperties
	{
		public HediffCompProperties_Regeneration()
		{
			this.compClass = typeof(HediffComp_Regeneration);
		}

		public Dictionary<int, float> regenerations = new Dictionary<int, float>();
	}
	public class HediffComp_Regeneration : HediffComp
	{
		public HediffCompProperties_Regeneration Props => (HediffCompProperties_Regeneration)this.props;
		public override void CompPostTick(ref float severityAdjustment)
		{
			if (this.Props.regenerations.TryGetValue(this.parent.CurStageIndex, out float num3))
			{
				num3 /= 60000f;
				if (num3 > 0f)
				{
					List<Hediff_Injury> tmpHediffInjuries = new List<Hediff_Injury>();
					this.Pawn.health.hediffSet.GetHediffs<Hediff_Injury>(ref tmpHediffInjuries, (Hediff_Injury h) => true);
					foreach (Hediff_Injury hediff_Injury2 in tmpHediffInjuries)
					{
						float num4 = Mathf.Min(num3, hediff_Injury2.Severity);
						num3 -= num4;
						hediff_Injury2.Heal(num4);
						this.Pawn.health.hediffSet.Notify_Regenerated(num4);
						if (num3 <= 0f)
						{
							break;
						}
					}
					if (num3 > 0f)
					{
						List<Hediff_MissingPart> tmpHediffMissing = new List<Hediff_MissingPart>();
						this.Pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>(ref tmpHediffMissing, (Hediff_MissingPart h) => h.Part.parent != null && !tmpHediffInjuries.Any((Hediff_Injury x) => x.Part == h.Part.parent)
						&& this.Pawn.health.hediffSet.GetFirstHediffMatchingPart<Hediff_MissingPart>(h.Part.parent) == null && this.Pawn.health.hediffSet.GetFirstHediffMatchingPart<Hediff_AddedPart>(h.Part.parent) == null);
						using (List<Hediff_MissingPart>.Enumerator enumerator3 = tmpHediffMissing.GetEnumerator())
						{
							if (enumerator3.MoveNext())
							{
								Hediff_MissingPart hediff_MissingPart = enumerator3.Current;
								BodyPartRecord part = hediff_MissingPart.Part;
								this.Pawn.health.RemoveHediff(hediff_MissingPart);
								Hediff hediff5 = this.Pawn.health.AddHediff(HediffDefOf.Misc, part, null, null);
								float partHealth = this.Pawn.health.hediffSet.GetPartHealth(part);
								hediff5.Severity = Mathf.Max(partHealth - 1f, partHealth * 0.9f);
								this.Pawn.health.hediffSet.Notify_Regenerated(partHealth - hediff5.Severity);
							}
						}
					}
				}
			}
		}
	}
}
