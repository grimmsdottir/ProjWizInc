using ProjWizInc.Core.Definitions.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Interfaces {
    public interface IReadJob : IRead {
        public int ActiveJobId { get; }
        public JobDefinition ActiveJobDefinition { get; }

    }
}
