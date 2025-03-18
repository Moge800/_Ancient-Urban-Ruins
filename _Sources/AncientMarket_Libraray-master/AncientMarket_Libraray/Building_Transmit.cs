using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class Building_Transmit : Building_Storage
    {
        public bool CanTransmit => this.PowerComp is CompPowerTrader comp && comp.PowerOn && this.receive != null;
        public override void Tick()
        {
            base.Tick();
            if (this.IsHashIntervalTick(10) && this.CanTransmit)
            {
                this.slotGroup.HeldThings.ToList().ListFullCopy().ForEach(x => 
                {
                    x.DeSpawn();
                    GenSpawn.Spawn(x, this.receive.Position, this.receive.Map);
                });
            }
        }
        public override void Notify_ReceivedThing(Thing newItem)
        {
            base.Notify_ReceivedThing(newItem);
            if (this.CanTransmit)
            {
               newItem.DeSpawn();
                GenSpawn.Spawn(newItem,this.receive.Position,this.receive.Map);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.receive, "receive");
        }

        public Building_Receive receive;
    }

    public class Building_Receive : Building_Storage
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.name, "name");
        }

        public string name;
    }
}
