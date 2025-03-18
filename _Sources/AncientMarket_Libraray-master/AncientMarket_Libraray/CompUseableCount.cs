using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class CompUseableCount : ThingComp
    {
        public CompPropertiesUseableCount Props => (CompPropertiesUseableCount)this.props;
        public override string CompInspectStringExtra()
        {
            return base.CompInspectStringExtra() + $"{this.Count} / {this.Props.useableCount}";
        }
        public int Count
        {
            get 
            {
                if (this.count == null)
                {
                    this.count = this.Props.useableCount;
                }
                return this.count.Value;
            }
        }
        public override bool AllowStackWith(Thing other)
        {
            return base.AllowStackWith(other) && other.TryGetComp<CompUseableCount>() is CompUseableCount comp && comp.count == this.count;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.count, "count");
        }

        public int? count = null;
    }

    public class CompPropertiesUseableCount : CompProperties
    {
        public CompPropertiesUseableCount() 
        {
            this.compClass = typeof(CompUseableCount);
        }

        public int useableCount = 2;
    }
}
