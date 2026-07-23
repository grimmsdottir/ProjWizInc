using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.State {
    public class StateRoot {
        public RecordRoot records;
        public TallyRoot tallies;
        public StateRoot() {
            records = new RecordRoot();
            tallies = new TallyRoot();
        }
    }
}
