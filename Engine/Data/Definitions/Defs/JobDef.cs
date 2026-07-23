using ProjWizInc.Engine.Data.Definitions.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions.Defs {
    public class JobDef : CompositeDefinition{
        public JobDef() : base() { }
        public JobDef(string key) : base(key) { }
    }
}
