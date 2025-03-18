using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AncientMarket_Libraray
{
    public class JobDriver_StartDialog : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch);
            yield return new Toil()
            {
                initAction = () =>
                {
                    if (this.TargetThingA.TryGetComp<CompDialogable>().Props.dialog is DialogTreeDef dialog)
                    {
                        Find.WindowStack.Add(dialog.CreateDialog(this.pawn, this.TargetThingA));
                    }
                }
                ,
                defaultCompleteMode = ToilCompleteMode.Delay
            };
            yield break;
        }
    }
}
