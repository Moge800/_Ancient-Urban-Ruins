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
    public class MapExit : PocketMapExit
    {
        public AMMapPortal MapEntrance => (AMMapPortal)this.entrance;
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
                for (int i = 0; i < this.CD.Count(); i++)
                {
                    Pawn p2 = this.CD.ToList()[i].Key;
                    this.CD[p2] = this.CD[p2] - 1;
                }
                this.CD.RemoveAll(c => c.Value <= 0);
            }
            if (this.IsHashIntervalTick(5) && this.entrance != null && this.Position.GetFirstPawn(this.Map) is Pawn p && p.Drafted)
            {
                p.DeSpawn();
                GenSpawn.Spawn(p, GetDestinationLocation(), this.entrance.Map); 
                this.OnEntered(p);
            }
        }
        public override void OnEntered(Pawn pawn)
        {
            base.OnEntered(pawn);
            if (pawn.GetLord() is Lord l && l.LordJob is LordJob_AssaultColony || pawn.mindState.duty?.def == DutyDefOf.AssaultColony)
            {
                this.PawnAndLords.SetOrAdd(pawn, pawn.GetLord());
            }
            this.MapEntrance.CD.SetOrAdd(pawn, 1200);
        }
        public virtual bool IsAllowed(Pawn pawn)
        {
            return !pawn.IsColonist || (this.MapEntrance.Map.Parent as MapParent_Custom == null || GameComponent_AncientMarket.GetComp.GetSchedule(pawn).allowedLevels.Contains(this.MapEntrance));
        }
        public override IntVec3 GetDestinationLocation()
        {
            return this.entrance.Position + (this.entrance.Rotation.FacingCell * (this.def.HasModExtension<ModExtension_Portal>() ? this.def.GetModExtension<ModExtension_Portal>().distance : 1));
        }

        public override Map GetOtherMap()
        {
            return this.parentMap;
        }
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
            Scribe_References.Look(ref this.parentMap, "parentMap");
        }

        public List<Pawn> pawnAndLords_p = new List<Pawn>();
        public List<Lord> pawnAndLords_l = new List<Lord>();
        private Dictionary<Pawn, Lord> pawnAndLords = new Dictionary<Pawn, Lord>();

        public List<Pawn> CDs_p = new List<Pawn>();
        public List<int> CDs_i = new List<int>();
        public Dictionary<Pawn, int> CDs = new Dictionary<Pawn, int>();
        public Map parentMap;
    }
}
