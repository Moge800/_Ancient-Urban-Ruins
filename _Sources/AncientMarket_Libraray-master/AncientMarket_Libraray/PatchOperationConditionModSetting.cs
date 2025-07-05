using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace AncientMarket_Libraray
{
    public class PatchOperationConditionModSetting : PatchOperation
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (this.math != null
                && AM_ModSetting.setting is AM_ModSetting setting
                && (setting.enablePatch != this.invert))
            {
                return this.math.Apply(xml);
            }
            return true;
        }

        public bool invert = false;
        public PatchOperation math;
    }
}
