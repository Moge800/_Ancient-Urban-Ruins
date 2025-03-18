using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AncientMarket_Libraray
{
    public class Lootbox : Building_Casket
    {
        public ModExtension_Lootbox Extension => this.def.GetModExtension<ModExtension_Lootbox>();
        public override Graphic Graphic
        {
            get
            {
                if (this.HasAnyContents)
                {
                    return base.Graphic;
                }
                if (this.openedGraphic == null)
                {
                    Graphic baseGraphic = base.Graphic;
                    if (this.Extension.openedGraphicdata != null)
                    {
                        this.openedGraphic = this.Extension.openedGraphicdata.Graphic;
                        return this.openedGraphic;
                    }

                    this.openedGraphic = base.Graphic;
                }
                return this.openedGraphic;
            }
        }
        public void OpenByNPC(Pawn pawn) 
        {
            this.innerContainer.TryTransferAllToContainer(pawn.inventory.innerContainer);
            this.contentsKnown = true;
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!inti) 
            {
                this.Extension.loots.RandomElement().root.Generate().ForEach(t => 
                {
                    if (t.Spawned) 
                    {
                        t.DeSpawn();
                    }
                    this.innerContainer.TryAddOrTransfer(t);
                });
                this.contentsKnown = false;
                inti = true;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.inti, "inti");
            Scribe_Values.Look(ref this.tickToOpen, "QE_LootBox_tickToOpen");
        }

        public int tickToOpen = 100;
        public bool inti = false;
        public Graphic openedGraphic = null;
    }
}