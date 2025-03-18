using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AncientMarket_Libraray
{
    public class VirtualMiner : Building
    {
        public ModExtension_VirutalMiner Extension => this.def.GetModExtension<ModExtension_VirutalMiner>();
        public override Graphic Graphic
        {
            get
            {
                if (this.graphicCards.Any() && this.graphicCards.Count <= this.Extension.graphicsData.Count) 
                {
                    return this.Extension.graphicsData[this.graphicCards.Count - 1].Graphic;
                }
                return base.Graphic;
            }
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var item in base.GetGizmos())
            {
                yield return item;
            }
            yield return new Command_Toggle
            {
                defaultLabel = "CommandToggleAllowAutoAddGraphicCard".Translate(),
                defaultDesc = "CommandToggleAllowAutoAddGraphicCardDesc".Translate(),
                icon = (this.allowedToAddCard ? TexCommand.ForbidOff : TexCommand.ForbidOn),
                isActive = () => true,
                toggleAction = delegate ()
                {
                    this.allowedToAddCard = !this.allowedToAddCard;
                }

            };
            if (Prefs.DevMode) 
            {
                yield return new Command_Action() 
                {
                    defaultLabel = "GenerateProducts",
                action = () => this.GenerateProducts()
                };
            }
            yield break;
        }
        public override string GetInspectString()
        {
            return base.GetInspectString() 
                + (this.graphicCards.NullOrEmpty() ? "NoGPU".Translate() : "VirtualMiningProgress".Translate(GenDate.TicksToDays(this.Extension.tickToProduce - this.progresss)))
                +"\n"+ "GPUCount".Translate(this.graphicCards.Count);
        }
        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {
            this.graphicCards.ForEach(t => GenSpawn.Spawn(t,this.Position,this.Map));
            base.Kill(dinfo, exactCulprit);
        }
        public override void Tick()
        {
            base.Tick();
            if (this.graphicCards.Any() && this.PowerComp is CompPowerTrader power && power.PowerOn)
            {
                this.progresss++;
                if (this.progresss > this.Extension.tickToProduce * (1 - (0.1f *this.graphicCards.Count)))
                {
                    this.progresss = 0;
                    GenerateProducts();
                }
            }
        }

        private void GenerateProducts()
        {
            foreach (var item in this.graphicCards)
            {
                ThingDefCountClass product = this.Extension.products.Any() ? this.Extension.products.RandomElement() : new ThingDefCountClass() { thingDef = ThingDefOf.Gold, count = 1 };
                Thing t = GenSpawn.Spawn(product.thingDef, this.Position, this.Map);
                t.stackCount = product.count;
                CompQuality compQuality = t.TryGetComp<CompQuality>();
                if (compQuality != null)
                {
                    compQuality.SetQuality(QualityUtility.GenerateQualityRandomEqualChance(), new ArtGenerationContext?(ArtGenerationContext.Colony));
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.progresss,"progress");
            Scribe_Values.Look(ref this.allowedToAddCard, "allowedToAddCard");
            Scribe_Collections.Look(ref this.graphicCards,"graphicCards",LookMode.Deep);
        }

        public bool allowedToAddCard = true;
        public int progresss = 0;
        public List<Thing> graphicCards = new List<Thing>();
    }
    public class GPU : ThingWithComps 
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (var item in base.GetFloatMenuOptions(selPawn))
            {
                yield return item;
            }
            if (selPawn.CanReserveAndReach(this,PathEndMode.Touch,Danger.Deadly) && selPawn.Map.listerThings.GetThingsOfType<VirtualMiner>().ToList().Find(
                m => selPawn.CanReserveAndReach(m, PathEndMode.Touch, Danger.Deadly) 
                && ((VirtualMiner)m).graphicCards.Count <= 3) is VirtualMiner miner) 
            {
                Job result = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("AM_AddGPU"), this, miner);
                result.count = 1;
                yield return new FloatMenuOption("AddThisToMiner".Translate(), () => selPawn.jobs.StartJob(result));
            }
            yield break;
        }
    }
    public class ModExtension_VirutalMiner :DefModExtension 
    {
        public List<GraphicData> graphicsData = new List<GraphicData>();
        public List<ThingDefCountClass> products = new List<ThingDefCountClass>();
        public int tickToProduce = 60000;
    }
}
