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
    public class ScenPart_GenerateCustomMap : ScenPart
    {
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, this.maps.Count * 30f + ScenPart.RowHeight);
            this.DrawButtonWithIcon(scenPartRect.y, () => this.DrawFloatMenu(DefDatabase<CustomMapDataDef>.AllDefsListForReading, d => this.maps.Add(d), d => d.label), () =>
            this.DrawFloatMenu(this.maps, d => this.maps.Remove(d), d => d.label), scenPartRect.width + 20f, 20);
            scenPartRect.y += 30f;
            this.maps.ForEach(m =>
            {
                Widgets.Label(scenPartRect, m.label);
                scenPartRect.y += 30f;
            });
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
        public void DrawButtonWithIcon(float y, Action addAction, Action removeAction, float x = 10f, float iconSize = 25f, float interval = 35f, Vector2? size = null)
        {
            if (Widgets.ButtonImage(new Rect(x, y, iconSize, iconSize), TexButton.Plus))
            {
                addAction();
            }
            if (Widgets.ButtonImage(new Rect(x + interval, y, iconSize, iconSize), TexButton.Delete))
            {
                removeAction();
            }
        }
        public override void PostMapGenerate(Map map)
        {
            base.PostMapGenerate(map);
            if (Find.TickManager.TicksGame < 5f)
            {
                CustomMapDataDef def = this.maps.RandomElement();
                MapGeneratingUtility.SpawnCustomMap(map, def, map.Center - new IntVec3(def.size.x / 2, 0, def.size.z / 2));
            }
        }

        public List<CustomMapDataDef> maps = new List<CustomMapDataDef>();
    }
}
