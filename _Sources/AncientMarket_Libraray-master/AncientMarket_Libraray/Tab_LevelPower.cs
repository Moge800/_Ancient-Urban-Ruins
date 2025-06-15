using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AncientMarket_Libraray
{
    public class Tab_LevelPower : ITab
    {
        public Tab_LevelPower()
        {
            this.size = new Vector2(400f, 300f);
            this.labelKey = "TabLevelPower";
            this.tutorTag = "LevelPower";
        }
        public CompPowerPlantLevel Comp
        {
            get
            {
                return (this.SelThing as ThingWithComps)?.TryGetComp<CompPowerPlantLevel>();
            }
        }
        protected override void FillTab()
        {
            if (this.Comp != null)
            {
                float y = 5f;
                float x = 5f;
                this.Comp.name = DrawLabelAndText_Line(y, "LevelPowerName".Translate(), ref this.Comp.name, x, 100f);
                y += 30f;
                bool mode = this.Comp.outputMode;
                Widgets.CheckboxLabeled(new Rect(x, y, 150f, 25f), "OutputMode".Translate(), ref this.Comp.outputMode);
                if (mode != this.Comp.outputMode)
                {
                    if (this.Comp.LinkedComp != null) 
                    {
                        this.Comp.LinkedComp.comp = null;
                        this.Comp.LinkedComp.linked = null;
                    }
                    this.Comp.linked = null;
                    this.Comp.comp = null;
                }
                y += 30f;
                if (this.Comp.outputMode)
                {
                    DrawLabelAndText_Line(y, "LevelPowerOutput".Translate(), ref this.Comp.targetPowerOutput, ref this.Comp.buffer, x, 100f);
                    y += 30f;
                    if (Widgets.ButtonText(new Rect(x, y, 150f, 25f), "SelectLinkedPower".Translate(this.Comp.LinkedComp?.name)))
                    {
                        Map map = this.SelThing.Map;
                        List<Map> targetMaps = new List<Map>();
                        List<Thing> things = new List<Thing>();
                        if (map != null)
                        {
                            if (map.Parent is MapParent_Custom custom)
                            {
                                targetMaps.Add(custom.sourceMap);
                            }
                            if (map.GetComponent<MapComponent_Submap>() is MapComponent_Submap comp)
                            {
                                comp.Submaps.ForEach(m =>
                                {
                                    if(m?.Map != null)
                                    targetMaps.Add(m.Map);
                                });
                            }
                        }
                        targetMaps.ForEach(m => things.AddRange(m.listerBuildings.allBuildingsColonist.FindAll(b => b.def == this.SelThing.def && b.TryGetComp<CompPowerPlantLevel>().outputMode != this.Comp.outputMode)));
                        if (things.Any())
                        {
                            DrawFloatMenu(things, t =>
                            {
                                this.Comp.Link(t.TryGetComp<CompPowerPlantLevel>());
                            }, t => t.TryGetComp<CompPowerPlantLevel>()?.name);
                        }
                    }
                    if (this.Comp.outputMode && Widgets.ButtonText(new Rect(x + 180f, y, 150f, 25f), "Apply".Translate()))
                    {
                        this.Comp.PowerOutput = -this.Comp.targetPowerOutput;
                    }
                }
            }
        }
        public static void DrawFloatMenu<T>(List<T> list, Action<T> action, Func<T, string> text, List<FloatMenuOption> extra = null, Func<T, bool> validator = null)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            if (extra != null)
            {
                options.AddRange(extra);
            }
            foreach (T t in list)
            {
                if (validator == null || validator(t))
                {
                    FloatMenuOption option = new FloatMenuOption(text(t), () =>
                    {
                        action(t);
                    });
                    options.Add(option);
                }
            }
            if (options.Any())
            {
                Find.WindowStack.Add(new FloatMenu(options));
            }
        }
        public static string DrawLabelAndText_Line(float y, string label, ref string text, float x = 0f, float width = 60f)
        {
            bool nullText = label.NullOrEmpty();
            if (!nullText)
            {
                Widgets.Label(new Rect(x, y, 350f, 25f), label);
            }
            text = Widgets.TextField(new Rect(nullText ? x + 5f : Text.CalcSize(label).x + x + 5f, y, width, 25f), text);
            return text;
        }

        public static void DrawLabelAndText_Line<T>(float y, string label, ref T text, ref string buffer, float x = 0f, float width = 60f) where T : struct
        {
            Widgets.Label(new Rect(x, y, 350f, 25f), label);
            Widgets.TextFieldNumeric<T>(new Rect(Text.CalcSize(label).x + x + 5f, y, width, 25f), ref text, ref buffer);
        }

        public CompPowerPlantLevel comp;
    }
}
