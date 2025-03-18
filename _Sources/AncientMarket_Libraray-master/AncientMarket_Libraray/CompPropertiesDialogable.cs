using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class CompPropertiesDialogable : CompProperties
    {
        public CompPropertiesDialogable()
        {
            this.compClass = typeof(CompDialogable);
        }

        public DialogTreeDef dialog;
    }
}
