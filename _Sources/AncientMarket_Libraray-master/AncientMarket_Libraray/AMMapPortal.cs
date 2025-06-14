using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace AncientMarket_Libraray
{
    public abstract class AMMapPortal : MapPortal
    {
        public virtual AMMapPortal MapEntrance => this;
        public Dictionary<Pawn, Lord> PawnAndLords 
        {
            get 
            {
                if (this.pawnAndLords == null)
                {
                    this.pawnAndLords = new Dictionary<Pawn, Lord>();
                }
                return this.pawnAndLords;
            }
        }
        public Dictionary<Pawn, int> CD
        {
            get
            {
                if (this.CDs == null)
                {
                    this.CDs = new Dictionary<Pawn, int>();
                }
                return this.CDs;
            }
        }
        public virtual bool IsAllowed(Pawn pawn)
        {
            return !pawn.IsColonist || GameComponent_AncientMarket.GetComp.GetSchedule(pawn).allowedLevels.Contains(this.MapEntrance);
        }
        public bool IsAvailable(Pawn pawn) 
        {
            return !this.CD.ContainsKey(pawn) && this.IsAllowed(pawn);
        }
        protected override void Tick()
        {
            base.Tick(); 
            if (this.PawnAndLords.Any())
            {
                List<Pawn> shouldRemove = new List<Pawn>();
                this.PawnAndLords.ToList().ForEach(p2 =>
                {
                    if (p2.Value != null && !p2.Value.ownedPawns.Contains(p2.Key))
                    {
                        p2.Value.AddPawn(p2.Key);
                    }
                    else
                    {
                        shouldRemove.Add(p2.Key);
                        if (p2.Value != null) 
                        {
                            p2.Value.numPawnsEverGained--;
                        }
                    }
                });
                this.PawnAndLords.RemoveAll(p3 => shouldRemove.Contains(p3.Key));
            }
            if (this.CD.Any())
            {
                for (int i = 0; i<this.CD.Count(); i++) 
                {
                    Pawn p = this.CD.ToList()[i].Key;
                    this.CD[p] = this.CD[p] - 1;
                }
                this.CD.RemoveAll(c => c.Value <= 0);
            }
        }
        public override string GetInspectString()
        {
            if (Prefs.DevMode)
            {
                StringBuilder result = new StringBuilder();
                this.CDs.ToList().ForEach(c => result.AppendLine(c.ToString() + ","));
                return result.ToString().Trim();
            }
            return base.GetInspectString();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref this.pawnAndLords, "pawnAndLords", LookMode.Reference, LookMode.Reference, ref this.pawnAndLords_p, ref this.pawnAndLords_l);
            Scribe_Collections.Look(ref this.CDs, "CDs", LookMode.Reference, LookMode.Value, ref this.CDs_p, ref this.CDs_i);
        }

        public List<Pawn> pawnAndLords_p = new List<Pawn>();
        public List<Lord> pawnAndLords_l = new List<Lord>();
        private Dictionary<Pawn, Lord> pawnAndLords = new Dictionary<Pawn, Lord>();

        public List<Pawn> CDs_p = new List<Pawn>();
        public List<int> CDs_i = new List<int>();
        public Dictionary<Pawn, int> CDs = new Dictionary<Pawn, int>();
    }
}
