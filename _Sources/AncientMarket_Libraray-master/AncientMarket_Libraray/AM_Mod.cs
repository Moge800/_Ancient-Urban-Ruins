using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AncientMarket_Libraray
{
    public class AM_Mod : Mod
    {
        public AM_Mod(ModContentPack content) : base(content)
        {
            this.setting = this.GetSettings<AM_ModSetting>();
        }
        public override string SettingsCategory()
        {
            return "AncientMarket".Translate();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            inRect.y += 30f;
            Widgets.CheckboxLabeled(new Rect(inRect.x, inRect.y, 300f, 30f), "EnableAICrossLevel".Translate(), ref this.setting.enableAICrossLevel);
            inRect.y += 50f;
            Widgets.CheckboxLabeled(new Rect(inRect.x, inRect.y, 300f, 30f), "EnableLandfill".Translate(), ref this.setting.enableLandfill);
        }
        public AM_ModSetting setting = null;
    }

    public class AM_ModSetting : ModSettings
    {
        public AM_ModSetting()
        {
            AM_ModSetting.setting = this;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.enableLandfill, "enableLandfill");
            Scribe_Values.Look(ref this.enableAICrossLevel, "enableAICrossLevel");
        }

        public bool enableLandfill = true;
        public bool enableAICrossLevel = true;
        public static AM_ModSetting setting;
    }
}
