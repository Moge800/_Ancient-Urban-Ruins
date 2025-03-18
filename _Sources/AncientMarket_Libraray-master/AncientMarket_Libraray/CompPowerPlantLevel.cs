using JetBrains.Annotations;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class CompPowerPlantLevel : CompPowerPlant
    {
        protected override float DesiredPowerOutput => this.outputMode ? -this.targetPowerOutput  : (this.LinkedComp == null || !this.LinkedComp.PowerOn) ? 0f : -this.LinkedComp.PowerOutput;
        public CompPowerPlantLevel LinkedComp 
        {
            get
            {
                if (this.comp == null)
                {
                    this.comp = this.linked?.TryGetComp<CompPowerPlantLevel>();
                }
                return this.comp;
            }
        }
        public void Link(CompPowerPlantLevel comp) 
        {
            this.linked = comp.parent;
            comp.linked = this.parent;
        }
        public override void UpdateDesiredPowerOutput()
        {
            base.PowerOutput = this.DesiredPowerOutput;
        }
        public override void SetUpPowerVars()
        {
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref this.linked, "linked");
            Scribe_Values.Look(ref this.targetPowerOutput, "targetPowerOutput");
            Scribe_Values.Look(ref this.outputMode, "outputMode");
            Scribe_Values.Look(ref this.name, "name");
            Scribe_Values.Look(ref this.buffer, "buffer");
        }

        public CompPowerPlantLevel comp;
        public string name = "Undefined";
        public bool outputMode = false;
        public Thing linked;
        public float targetPowerOutput = 0f;
        public string buffer;
    }
}
