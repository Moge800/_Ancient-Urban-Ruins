using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Verse;

namespace AncientMarket_Libraray
{
    public class DialogTreeDef : Def
    {
        public bool CheckRequiredThings(List<LootThingData> requiredThings, List<Thing> things, out ThingDef def, out int count, out int limit)
        {
            Dictionary<ThingDef, int> counts = new Dictionary<ThingDef, int>();
            requiredThings.ForEach(d => counts.Add(((CQFThingDefCount)d).thing, d.count.min));
            foreach (Thing t in things)
            {
                if (counts.ContainsKey(t.def))
                {
                    counts[t.def] -= t.stackCount;
                }
            }
            if (counts.ToList().Find(c => c.Value >= 1) is KeyValuePair<ThingDef, int> thing && thing.Key != null)
            {
                def = thing.Key;
                limit = requiredThings.Find(t => t is CQFThingDefCount tc && tc.thing == thing.Key).count.min;
                count = limit - thing.Value;
                return false;
            }
            def = null;
            count = 0;
            limit = 0;
            return true;
        }
        public void ConsumeRequiredThings(Pawn interviwer, Pawn interviwee, List<LootThingData> requiredThings)
        {
            if (requiredThings.Any() && interviwer != null)
            {
                if (interviwer.Map.IsPlayerHome)
                {
                    requiredThings.ForEach(d =>
                    {
                        this.ConsumeThings(((CQFThingDefCount)d).thing, d.count.min, interviwer.Map, null);
                    });
                }
                else
                {
                    Dictionary<ThingCategoryDef, int> categoryAndCount = new Dictionary<ThingCategoryDef, int>();
                    foreach (LootThingData data in requiredThings)
                    {
                        if (data is CQFThingDefCount tData)
                        {
                            interviwer?.inventory?.innerContainer.Take(interviwer?.inventory.innerContainer.ToList().Find(i => i.def == tData.thing), tData.count.min).Destroy();
                        }
                    }
                }
            }
        }
        public void ConsumeThings(ThingDef def, int count, Map map, Pawn receiver = null)
        {
            foreach (Thing t in AllConsumableThingForDef(def, map))
            {
                int spliteCount = t.stackCount <= count ? t.stackCount : count;
                Thing thing = t.SplitOff(spliteCount);
                count -= spliteCount;
                if (receiver == null)
                {
                    thing.Destroy();
                }
                else
                {
                    receiver.inventory.TryAddAndUnforbid(thing);
                }
                if (count <= 0)
                {
                    break;
                }
            };
        }
        public List<Thing> AllConsumableThing(Map map)
        {
            return map.listerThings.AllThings.FindAll(t => !t.IsForbidden(Faction.OfPlayer) && !t.Position.Fogged(map)).ListFullCopy();
        }
        public List<Thing> AllConsumableThingForDef(ThingDef def, Map map)
        {
            return map.listerThings.ThingsOfDef(def).FindAll(t => !t.IsForbidden(Faction.OfPlayer) && !t.Position.Fogged(map)).ListFullCopy();
        }
        public void AddIdleNode(DialogNode node)
        {
            this.idleNodes.Add(node);
            node.subNodeIndexs.ForEach(i =>
            {
                DialogNode subNode = this.nodeMoulds[i];
                if (this.IsIdleNode(subNode))
                {
                    this.AddIdleNode(subNode);
                }
            });
        }
        public bool IsIdleNode(DialogNode node)
        {
            return !this.nodeMoulds.Values.ToList().Exists(x => x.options.Exists(o => o.results.Exists(r => r.nextIndex == node.index)));
        }
        public void ChangeNextNodeToOtherNode(DialogNode parent, DialogNode newNode, DialogResult result, bool isNewNode = false)
        {
            DialogNode oldNode = result.nextIndex == null ? null : this.nodeMoulds[result.nextIndex.Value];
            if (oldNode != null && parent.subNodeIndexs.Contains(oldNode.index.Value) && !parent.options.Exists(o => o.results.Exists(r => r != result && r.nextIndex == result.nextIndex)))
            {
                parent.subNodeIndexs.Remove(oldNode.index.Value);
                this.AddIdleNode(oldNode);
            }
            if (newNode != null)
            {
                if (newNode.index != 0 && newNode != parent && (this.idleNodes.Contains(newNode) || isNewNode))
                {
                    parent.subNodeIndexs.Add(newNode.index.Value);
                }
                if (this.idleNodes.Contains(newNode))
                {
                    this.idleNodes.Remove(newNode);
                }
                result.nextIndex = newNode.index.Value;
            }
            else
            {
                result.nextIndex = null;
            }
        }
        public DialogNode CreateNewNode(DialogNode parent)
        {
            DialogNode result = new DialogNode(this.curIndex);
            this.nodeMoulds.Add(this.curIndex, result);
            this.curIndex++;
            if (parent != null)
            {
                result.parentIndex = parent.index;
            }
            return result;
        }
        public string GetDialogText(string text, Thing interviewer, Thing interviewee)
        {
            TaggedString result =text.Translate();
            return result;
        }
        public Dialog_NodeTree CreateDialog(Thing interviewer, Thing interviewee)
        {
            Dictionary<int, DiaNode> nodes = new Dictionary<int, DiaNode>();
            string interviewrText = interviewer is Pawn ? interviewer.Label : ((Pawn)interviewer).Name.ToStringFull;
            foreach (KeyValuePair<int, DialogNode> nodeMould in this.nodeMoulds)
            {
                List<string> texts = new List<string>();
                texts.Add(nodeMould.Value.text);
                texts.AddRange(nodeMould.Value.extraText);
                string text = this.GetDialogText(texts.RandomElement(), interviewer, interviewee);
                DiaNode node = new DiaNode(text);
                nodeMould.Value.options.ForEach(o =>
                {
                    DiaOption option = new DiaOption(this.GetDialogText(o.text, interviewer, interviewee));
                    if (o.requiredThings.Any())
                    {
                        if (interviewer.Map.IsPlayerHome)
                        {
                            List<Thing> things = this.AllConsumableThing(interviewer.Map).ToList();
                            if (interviewer is Pawn p && p.inventory != null)
                            {
                                things.AddRange(p.inventory.innerContainer.InnerListForReading);
                            }
                            if (!this.CheckRequiredThings(o.requiredThings, things, out ThingDef def, out int count, out int limit))
                            {
                                option.Disable("NoRequiredTHing".Translate(def, count, limit));
                            }
                        }
                        else
                        {
                            ThingDef def = null;
                            int count = 0;
                            int limit = 0;
                            if (!(interviewer is Pawn p) || p.inventory == null || !this.CheckRequiredThings(o.requiredThings, ((Pawn)interviewer).inventory.innerContainer.InnerListForReading, out def, out count, out limit))
                            {
                                option.Disable("NoRequiredTHing".Translate(def, count, limit));
                            }
                        }
                    }
                    option.action = () =>
                    {
                        Dictionary<string, TargetInfo> targets = new Dictionary<string, TargetInfo>();
                        targets.Add("Interviewer", interviewer);
                        targets.Add("Interviewee", interviewee);

                        o.ProduceResult(interviewee, interviewer).actions.ForEach(a => a.Work(targets));
                        if (o.removeDialogAfterSelect)
                        {
                            interviewee.TryGetComp<CompDialogable>().dialogued = true;
                        }
                        this.ConsumeRequiredThings(interviewer as Pawn, interviewee as Pawn, o.requiredThings);

                    };
                    if (!o.hideWhenDisabled || !option.disabled)
                    {
                        node.options.Add(option);
                    }
                });
                nodes.Add(nodeMould.Key, node);
            }
            foreach (KeyValuePair<int, DiaNode> node in nodes)
            {
                node.Value.options.ForEach(o =>
                {
                    string text = (string)(o.GetType().GetField("text", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o));
                    int? nextIndex = this.nodeMoulds[node.Key]?.options?.Find(x => this.GetDialogText(x.text, interviewer, interviewee) == text)?.ProduceResult(interviewee, interviewer).nextIndex;
                    if (nextIndex == null)
                    {
                        o.resolveTree = true;
                        return;
                    }
                    o.link = nodes[nextIndex.Value];
                });
            }
            if (!nodes.Any())
            {
                Log.Error("Create dialog error:Null node");
                return null;
            }
            string title = this.GetDialogText(this.title, interviewer, interviewee);
            Dialog_NodeTree result = new Dialog_NodeTree(nodes.First().Value, false, false, title);
            return result;
        }


        public string title = "DefaultDialogKey";
        public string dialogReportKey = "DefaultDialogKey";
        public bool requireNonHostile = true;
        public int curIndex = 1;
        public List<DialogNode> idleNodes = new List<DialogNode>();
        public Dictionary<int, DialogNode> nodeMoulds = new Dictionary<int, DialogNode>() { [0] = new DialogNode(0) };
    }
    public class DialogNode
    {
        public DialogNode()
        {
        }
        public DialogNode(int index)
        {
            this.index = index;
        }
        public DialogNode(int index, DialogNode parent)
        {
            this.index = index;
            parent.subNodeIndexs.Add(this.index.Value);
        }
        public float GetRequiredSpace(DialogTreeDef tree)
        {
            float result = 0f;
            this.options.ForEach(o => result += o.GetRequiredSpace(tree));
            return Math.Max(result, 40f);
        }


        public string text = "Default";
        public List<string> extraText = new List<string>();
        public int? index = null;
        public int? parentIndex = null;
        public List<DialogOption> options = new List<DialogOption>();
        public List<int> subNodeIndexs = new List<int>();
    }
    public class DialogOption
    {
        public DialogResult ProduceResult(Thing target, Thing interviwer)
        {
            Dictionary<string, TargetInfo> targets = new Dictionary<string, TargetInfo>();
            targets.Add("Interviewee", target);
            targets.Add("Interviewer", interviwer);
            return this.results.First();
        }
        public float GetRequiredSpace(DialogTreeDef tree)
        {
            float result = 0f;
            List<DialogNode> subNodes = new List<DialogNode>();
            foreach (KeyValuePair<int, DialogNode> node in tree.nodeMoulds)
            {
                if (this.results.Exists(r => r.nextIndex == node.Key))
                {
                    subNodes.Add(node.Value);
                }
            }
            foreach (DialogNode node in subNodes)
            {
                result += node.GetRequiredSpace(tree);
            }
            return Math.Max(result, 40f);
        }
        public string text = "Default";
        public bool hideWhenDisabled = false;
        public bool removeDialogAfterSelect = false;
        public List<DialogResult> results = new List<DialogResult>() { new DialogResult() };
        public List<LootThingData> requiredThings = new List<LootThingData>();
        //public bool requiredThingsWillBeGivenToInterviewer = false;
    }
    public class DialogResult
    {

        public string resultName = "Undefined";
        public List<CQFAction> actions = new List<CQFAction>();
        public int? nextIndex = null;
    }

    public abstract class CQFAction : IExposable
    {
        public abstract void ExposeData();
        public abstract void Work(Dictionary<string, TargetInfo> targets);
    }
    public abstract class LootThingData : IExposable
    {
        public abstract ThingRequest GetRequest();
        public virtual XElement SaveToXElement(string nodeName)
        {
            XElement result = new XElement(nodeName);
            result.SetAttributeValue("Class", this.GetType().FullName);
            if (this.stuff != null)
            {
                result.Add(new XElement("stuff", this.stuff.defName));
            }
            result.Add(new XElement("count", this.count.ToString()));
            return result;
        }
        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref this.count, "QE_ThingDefCountRangeWithBuffer_count");
            Scribe_Defs.Look(ref this.stuff, "QE_ThingDefCountRangeWithBuffer_stuff");
            Scribe_Values.Look(ref this.bufferMin, "QE_ThingDefCountRangeWithBuffer_bufferMin");
            Scribe_Values.Look(ref this.bufferMax, "QE_ThingDefCountRangeWithBuffer_bufferMax");
        }

        public string bufferMin;
        public string bufferMax;
        public ThingDef stuff = null;
        public IntRange count = new IntRange(1, 1);
    }
    public class CQFThingDefCount : LootThingData
    {
        public override ThingRequest GetRequest()
        {
            return ThingRequest.ForDef(this.thing);
        }

        public override string ToString()
        {
            return this.stuff?.label + " " + this.thing?.label + this.count.ToString();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref this.thing, "QE_ThingDefCountRangeWithBuffer_thing");
        }

        public ThingDef thing;
    }
    public class CQFAction_FactionToPlayer : CQFAction
    {
        public override void Work(Dictionary<string, TargetInfo> targets)
        {
            targets.ToList().ForEach(t =>
            {
                if (t.Value.Thing is Thing thing && thing.def.CanHaveFaction)
                {
                    thing.SetFaction(Faction.OfPlayer);
                }
            });
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref this.faction, "CQFAction_Faction_faction");
        }

        public FactionDef faction;
    }
}
