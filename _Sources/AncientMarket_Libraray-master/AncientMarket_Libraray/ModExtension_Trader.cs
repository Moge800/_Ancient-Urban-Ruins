using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class ModExtension_Trader : DefModExtension
    {
        public TraderKindDef tradeKind;
        public int intervalToUpdateGoods = 60000;
        public bool updateGoods = false;
        public bool leaveGoods = false;
    }
}