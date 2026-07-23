using ProjWizInc.Core.Data.Blueprints.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Records.Interfaces {
    public interface IReadJob : IRead {
        public int ActiveJobId { get; }
        public JobBlueprint ActiveJobDefinition { get; }

    }
}
