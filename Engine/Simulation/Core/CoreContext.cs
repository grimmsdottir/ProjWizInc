using ProjWizInc.Engine.Simulation.Events;
using ProjWizInc.Engine.Simulation.Registries;
using ProjWizInc.Engine.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Simulation.Core {
    public class CoreContext {
        private DefinitionRegistry _definitionRegistry;
        private StateRoot _stateRoot;
        private EventHub _eventHub;
        private HeartbeatManager _heartbeatManager;

        public CoreContext(
            DefinitionRegistry definitionRegistry, 
            StateRoot stateRoot,
            HeartbeatManager heartbeatManager
            ) {
            _definitionRegistry = definitionRegistry;
            _stateRoot = stateRoot;
            _heartbeatManager = heartbeatManager;
        }
        public void Start() {
            _heartbeatManager.Start();
        }
        public long GetTicks() {
            return _stateRoot.records.Time.GetTime();
        }
    }
}
