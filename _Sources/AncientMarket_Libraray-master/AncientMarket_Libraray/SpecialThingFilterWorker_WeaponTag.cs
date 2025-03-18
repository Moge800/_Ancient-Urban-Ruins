using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AncientMarket_Libraray
{
    public abstract class SpecialThingFilterWorker_WeaponTag : SpecialThingFilterWorker
    {
        public abstract string Tag { get; }
        public override bool AlwaysMatches(ThingDef def)
        {
            return def?.weaponTags == null || !def.weaponTags.Contains(this.Tag);
        }
        public override bool Matches(Thing t)
        {
            return this.AlwaysMatches(t.def);
        }
    }

    public class SpecialThingFilterWorker_IndustrialGunAdvanced : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "IndustrialGunAdvanced";
    }
    public class SpecialThingFilterWorker_Gun : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "Gun";
    }
    public class SpecialThingFilterWorker_WWGuns : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "WWGuns";
    }
    public class SpecialThingFilterWorker_SpacerGun : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "SpacerGun";
    }
    public class SpecialThingFilterWorker_SimpleGun : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "SimpleGun";
    }
    public class SpecialThingFilterWorker_Mg15 : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "Mg15";
    }
    public class SpecialThingFilterWorker_AssaultRifle : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "AssaultRifle";
    }
    public class SpecialThingFilterWorker_Auto : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "Auto";
    }
	public class SpecialThingFilterWorker_AMGuns : SpecialThingFilterWorker_WeaponTag
    {
        public override string Tag => "AMGuns";
    }
}
