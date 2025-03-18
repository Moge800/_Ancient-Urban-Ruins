using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientMarket_Libraray
{
    public class CompProperties_TransporterRequiredDialog : CompProperties_Transporter
    {
        public CompProperties_TransporterRequiredDialog() 
        {
            this.compClass = typeof(CompTransporterRequiredDialog);
        }
    }
}
