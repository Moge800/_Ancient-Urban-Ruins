using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public class GameComponent_AncientMarket : GameComponent
    {
        public GameComponent_AncientMarket(Game game) { }
        public static GameComponent_AncientMarket GetComp => Current.Game.GetComponent<GameComponent_AncientMarket>();
        public LevelSchedule GetSchedule(Pawn pawn)
        {
            if (this.schedules == null) 
            {
                this.schedules = new Dictionary<Pawn, LevelSchedule>();
            }
            if (!pawn.RaceProps.Humanlike && !pawn.IsColonist) 
            {
                return null;
            }
            if (!this.schedules.ContainsKey(pawn))
            {
                LevelSchedule levelSchedule = new LevelSchedule();
                levelSchedule.rootMap = AMUtility.GetRootMap(pawn.Map);
                AMUtility.GetSubMaps(levelSchedule.rootMap).ForEach(m =>
                {
                    if (m.entrance != null)
                    {
                        levelSchedule.allowedLevels.Add(m.entrance);
                    }
                });
                this.schedules.Add(pawn, levelSchedule);
            }
            if (this.schedules[pawn] is LevelSchedule schedule) 
            {
                Map root = AMUtility.GetRootMap(pawn.Map);
                if (schedule.rootMap == null)
                {
                    schedule.rootMap = root;
                }
                if (schedule.rootMap != root)
                {
                    schedule.rootMap = root;
                    schedule.allowedLevels.Clear();
                    schedule.workLevel = null;
                    schedule.joyLevel = null;
                    schedule.sleepLevel = null;
                    AMUtility.GetSubMaps(schedule.rootMap).ForEach(m =>
                    {
                        if (m.entrance != null)
                        {
                            schedule.allowedLevels.Add(m.entrance);
                        }
                    });
                }
            }
            return this.schedules[pawn];
        }
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (this.schedules_CD.Any()) 
            {
                foreach (var s in schedules_CD)
                {
                    s.CD--;
                } 
                this.schedules_CD.RemoveAll(s => s.CD <= 0);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                this.schedules.RemoveAll(s => s.Key == null || s.Key.Dead || s.Key.Destroyed);
            }
            Scribe_Collections.Look(ref this.schedules, "schedules", LookMode.Reference, LookMode.Deep, ref this.schedules_p, ref this.schedules_s);
        }

        public List<LevelSchedule> schedules_CD = new List<LevelSchedule>();

        public List<Pawn> schedules_p = new List<Pawn>();
        public List<LevelSchedule> schedules_s = new List<LevelSchedule>();
        public Dictionary<Pawn, LevelSchedule> schedules = new Dictionary<Pawn, LevelSchedule>();
    }

    public class LevelSchedule : IExposable
    {
        public LevelSchedule()
        {
            for (int i = 0; i < 24; i++)
            {
                this.timeSchedule.Add(false);
            }
        }
        public List<AMMapPortal> AllowedLevels 
        {
            get 
            {
                if (this.allowedLevels == null) 
                {
                    this.allowedLevels = new List<AMMapPortal>();
                }
                return this.allowedLevels;
            }
        }
        public void ExposeData()
        {
            Scribe_References.Look(ref this.rootMap, "rootMap");
            Scribe_References.Look(ref this.sleepLevel, "sleepLevel");
            Scribe_References.Look(ref this.workLevel, "workLevel");
            Scribe_References.Look(ref this.joyLevel, "joyLevel");
            Scribe_Collections.Look(ref this.timeSchedule, "timeSchedule",LookMode.Value);
            Scribe_Collections.Look(ref this.allowedLevels, "allowedLevels", LookMode.Reference);
        }

        public int CD;

        public Map rootMap;
        public List<bool> timeSchedule = new List<bool>();
        public List<AMMapPortal> allowedLevels = new List<AMMapPortal>();
        public AMMapPortal sleepLevel;
        public AMMapPortal workLevel;
        public AMMapPortal joyLevel;
    }
}
