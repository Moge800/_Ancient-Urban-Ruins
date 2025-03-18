using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AncientMarket_Libraray
{
    public class Window_AllowLevel : Window
    {
        public Window_AllowLevel(Pawn pawn) 
        {
            this.doCloseX = true;
            this.pawn = pawn;
        }
        public override void DoWindowContents(Rect inRect)
        {
            if (this.pawn != null && this.pawn.Spawned && GameComponent_AncientMarket.GetComp is GameComponent_AncientMarket comp)
            {
                Widgets.BeginScrollView(inRect, ref this.scrollPos, new Rect(0f, 0f, inRect.width, this.height));
                LevelSchedule schedule = comp.GetSchedule(this.pawn);
                float x = 5f;
                float y = 10f;
                List<MapParent_Custom> submaps = AMUtility.GetSubMaps(AMUtility.GetRootMap(this.pawn.Map));
                List<MapParent_Custom> subMapsForAllow = submaps.FindAll(m => !schedule.AllowedLevels.Contains(m.entrance));
                if (Widgets.ButtonText(new Rect(x, y, 150f, 25f), "SleepLevel".Translate(schedule.sleepLevel?.Label), false))
                {
                    this.DrawFloatMenu(submaps, (m) => schedule.sleepLevel = m.entrance, (m) => m.entrance.Label, new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("NotSet".Translate(),() => schedule.sleepLevel = null)
                    });
                }
                y += 30f;
                if (Widgets.ButtonText(new Rect(x, y, 150f, 25f), "WorkLevel".Translate(schedule.workLevel?.Label),false))
                {
                    this.DrawFloatMenu(submaps, (m) => schedule.workLevel = m.entrance, (m) => m.entrance.Label,new List<FloatMenuOption>() 
                    {
                        new FloatMenuOption("NotSet".Translate(),() => schedule.workLevel = null)
                    });
                }
                y += 30f;
                if (Widgets.ButtonText(new Rect(x, y, 150f, 25f), "JoyLevel".Translate(schedule.joyLevel?.Label),false))
                {
                    this.DrawFloatMenu(submaps, (m) => schedule.joyLevel = m.entrance, (m) => m.entrance.Label, new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("NotSet".Translate(),() => schedule.joyLevel = null)
                    });
                }
                y += 30f;
                schedule.AllowedLevels.ForEach(l =>
            {
                if (l != null)
                {
                    Text.Font = GameFont.Medium;
                    Widgets.Label(new Rect(x, y, 350f, 35f), l.Label);
                    y += 40f;
                }
            });
                Text.Font = GameFont.Small;
                Vector2 size = new Vector2(120f, 35f);
                if (Widgets.ButtonText(new Rect(x + 5f, y, size.x, size.y), "Add".Translate()) && subMapsForAllow.Any())
                {
                    this.DrawFloatMenu(subMapsForAllow, (m) => schedule.AllowedLevels.Add(m.entrance), (m) => m.entrance.Label);
                }
                if (Widgets.ButtonText(new Rect(inRect.width - 5f - size.x, y, size.x, size.y), "Remove".Translate()) && schedule.AllowedLevels.Any())
                {
                    this.DrawFloatMenu(schedule.AllowedLevels, (d) => schedule.AllowedLevels.Remove(d), (d) => d.Label);
                }
                y += size.y + 5f;
                Widgets.EndScrollView();
                this.height = y;
            }
        }

        public void DrawFloatMenu<T>(List<T> list, Action<T> action, Func<T, string> text, List<FloatMenuOption> extra = null, Func<T, bool> validator = null)
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



        public Pawn pawn;
        public Vector2 scrollPos = Vector2.zero;
        public float height = 0f;
    }
}
