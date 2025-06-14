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
    public class MapExit : AMMapPortal
    {
        public override AMMapPortal MapEntrance => this.entrance;
        protected override void Tick()
        {
            base.Tick();
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
            this.entrance.CD.SetOrAdd(pawn, 1200);
        }
        public override bool IsAllowed(Pawn pawn)
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.parentMap, "parentMap");
            Scribe_References.Look(ref this.entrance, "entrance");
        }
        public Map parentMap;
        public MapEntrance entrance;
    }
}
