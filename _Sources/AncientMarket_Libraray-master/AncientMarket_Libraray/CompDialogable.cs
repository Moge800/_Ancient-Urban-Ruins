using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse;

namespace AncientMarket_Libraray
{
    public class CompDialogable : ThingComp
    {
        public CompPropertiesDialogable Props => (CompPropertiesDialogable)props;
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (!this.dialogued)
            {
                DialogTreeDef def = this.Props.dialog;
                if (selPawn.CanReserveAndReach(this.parent,PathEndMode.Touch,Danger.Deadly))
                {
                   yield return new FloatMenuOption("AM_Dialog", () =>
                   {
                       selPawn.jobs.StopAll();
                       Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("AM_StartDialog"), this.parent);
                       selPawn.jobs.StartJob(job);
                   });
                }
            }
            yield break;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.dialogued, "dialogued");
        }

        public bool dialogued = false;
    }
}
