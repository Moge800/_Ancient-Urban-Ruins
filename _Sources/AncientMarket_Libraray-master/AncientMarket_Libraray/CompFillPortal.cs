using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AncientMarket_Libraray
{
    public class CompPropertiesFillPortal : CompProperties
    {
        public CompPropertiesFillPortal()
        {
            this.compClass = typeof(CompFillPortal);
        }

        public ThingDef filledBuilding;
        public int fillingWorkAmount;
        public string gizmoPath;

        public string gizmoName;
        public string gizmoDesc;

        public string disgizmoName;
        public string disgizmoDesc;
    }

    public class CompFillPortal : ThingComp
    {
        public CompPropertiesFillPortal Props => (CompPropertiesFillPortal)this.props;
        public Texture2D GizmoIcon 
        {
            get
            {
                if (this.gizmo == null) 
                {
                    this.gizmo = ContentFinder<Texture2D>.Get(this.Props.gizmoPath);
                }
                return this.gizmo;
            }
        }
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (this.planFill && selPawn.CanReserveAndReach(this.parent,PathEndMode.Touch,Danger.Deadly))
            {
                yield return new FloatMenuOption("FillEntrance".Translate(),() => selPawn.jobs.StartJob(JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("AM_FillEntrance"),this.parent)));
            }
            yield break;
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (AM_ModSetting.setting.enableLandfill) 
            {
            yield return new Command_Action()
            {
                defaultLabel = this.planFill ? this.Props.disgizmoName : this.Props.gizmoName,
                defaultDesc = this.planFill ? this.Props.disgizmoDesc : this.Props.gizmoDesc,
                icon = this.GizmoIcon,
                action = () => this.planFill = !this.planFill
            };
        }


            yield break;
        }
        public void Fill() 
        {
            IntVec3 pos = this.parent.Position;
            Map map = this.parent.Map;
            this.parent.Destroy();
            GenSpawn.Spawn(this.Props.filledBuilding,pos,map);
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.planFill, "planFill");
        }

        
        public Texture2D gizmo;

        public bool planFill = false;
    }

    public class JobDriver_FillEntrance : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.TargetThingA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            CompFillPortal comp = this.TargetThingA.TryGetComp<CompFillPortal>();
            yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch);
            Toil toil = Toils_General.WaitWith(TargetIndex.A, comp.Props.fillingWorkAmount, true);
            toil.AddPreTickAction(() => this.pawn.rotationTracker.FaceTarget(this.TargetThingA));
            yield return toil;
            yield return new Toil() { initAction = () => 
            {
                comp.Fill();
            }, defaultCompleteMode = ToilCompleteMode.Delay };
            yield break;
        }
    }
}