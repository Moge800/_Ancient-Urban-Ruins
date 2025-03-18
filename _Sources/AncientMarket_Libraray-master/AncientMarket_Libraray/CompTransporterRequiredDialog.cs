using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class CompTransporterRequiredDialog : CompTransporter
    {
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (this.parent.GetComp<CompDialogable>().dialogued)
            {
                foreach (Gizmo g in base.CompGetGizmosExtra())
                {
                    yield return g;
                }
            }
            yield break;
        }
    }
}