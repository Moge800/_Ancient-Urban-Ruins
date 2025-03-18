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
    public class IngestionOutcomeDoer_RestoreBodyPart : IngestionOutcomeDoer
    {
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            List<Hediff_MissingPart> missings = new List<Hediff_MissingPart>();
            pawn.health.hediffSet.GetHediffs<Hediff_MissingPart>(ref missings);
            if (missings.Any()) 
            {
                missings.SortStable((m, m2) => pawn.health.hediffSet.GetPartHealth(m.Part) > pawn.health.hediffSet.GetPartHealth(m2.Part) ? -1 : 1);
                Hediff_MissingPart hediff = missings.First();
                BodyPartRecord part = hediff.Part;
                pawn.health.RestorePart(part);
                Hediff hediff5 = pawn.health.AddHediff(HediffDefOf.Misc, part, null, null); 
                float partHealth = pawn.health.hediffSet.GetPartHealth(part);
                hediff5.Severity = partHealth - 1f;
            }

            if (ingested.TryGetComp<CompUseableCount>() is CompUseableCount comp && comp.Count > 1) 
            {
                Thing t = ThingMaker.MakeThing(ingested.def);
                t.TryGetComp<CompUseableCount>().count = comp.count - 1;
                pawn.inventory.TryAddItemNotForSale(t);
            }
        }
    }
}
