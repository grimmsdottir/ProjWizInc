using ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions.Specifications {
    public class TickProgressionSpec : ISpecification{
        public int FinishTicks {  get; init; }
        public TickProgressionSpec (int finishTicks) {
            FinishTicks = finishTicks;
        }
    }
}
