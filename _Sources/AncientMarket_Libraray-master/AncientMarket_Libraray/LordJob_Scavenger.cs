using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AncientMarket_Libraray
{
    public class LordJob_Scavenger : LordJob
    {
        public override StateGraph CreateGraph()
        {
            StateGraph g = new StateGraph();
            g.StartingToil = new LordToil_Scavenger();
            return g;
        }
    }
    public class LordToilData_Scavenger : LordToilData
    {
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref this.annoyed, "annoyed",LookMode.Reference);
        }

        public List<Pawn> annoyed = new List<Pawn>();
    }
    public class LordToil_Scavenger : LordToil
    {
        public LordToil_Scavenger() { this.data = new LordToilData_Scavenger(); }
        public LordToilData_Scavenger Data 
        {
            get 
            {
                if (this.data == null) 
                {
                this.data = new LordToilData_Scavenger();
                }
                return (LordToilData_Scavenger)this.data;
            }
        }
        public override void Notify_PawnDamaged(Pawn victim, DamageInfo dinfo)
        {
            base.Notify_PawnDamaged(victim, dinfo);
            if (dinfo.Instigator is Pawn pawn && pawn.IsPlayerControlled)
            {
                this.Data.annoyed.Add(victim);
                victim.mindState.duty = new PawnDuty(DutyDefOf.AssaultColony);
            }
        }
        public override void UpdateAllDuties()
        {
            DutyDef duty = DefDatabase<DutyDef>.GetNamed("AM_Scavenger");
            this.lord.ownedPawns.ForEach(p => p.mindState.duty = new PawnDuty(this.Data.annoyed.Contains(p) ? DutyDefOf.AssaultColony : duty, this.Map.AllCells.ToList().RandomElement()));
        }

    }
}