using ProjWizInc.Engine.Data.Definitions;
using ProjWizInc.Engine.Simulation.Core;
using ProjWizInc.Engine.Simulation.Events;
using ProjWizInc.Engine.Simulation.Persistence;
using ProjWizInc.Engine.Simulation.Registries;
using ProjWizInc.Engine.State;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ProjWizInc.Engine.Simulation.Bootstrapper {
    public static partial class Bootstrapper {
        /* Extra thicc and heavy class for constructing the CoreContext, Registry and just about anything that needs construction
         * 
         */
        //simple DTO for letting the parser know what to look for

        private const string DEFS_FOLDER = "defs";
        private const string SAVE_FOLDER = "save";
        //standard function for creating the CoreContext, reading from files and stuff
        public static CoreContext CreateCoreContext(bool isRaw, bool isNew) {
            DefinitionCatalog masterCatalog = new DefinitionCatalog();
            StateRoot stateRoot = new StateRoot();
            if (isRaw) {
                masterCatalog = GetRawDefs();
            } else {
                //TODO: the baked path that loads data from a bin rather than various jsons
            }
            if (isNew) {
                stateRoot = new StateRoot();
            } else {
                //TODO: save file loading
            }
            return CreateCoreContext(masterCatalog, stateRoot);
        }
        public static CoreContext CreateCoreContext(
            DefinitionCatalog definitionCatalog,
            StateRoot stateRoot
            ) {
            //step 1 data stuff
            DefinitionRegistry registry = CreateDefinitionRegistry(definitionCatalog);
            ResolveDefinitionRegistryLinks(registry);
            //step 2 infra stuff
            HeartbeatManager heartbeatManager = new HeartbeatManager(stateRoot.records.Time);
            EventHub eventHub = new EventHub();
            EventPublisher eventPublisher = new EventPublisher(eventHub,stateRoot.records.Economy);
            //step 3 domain stuff


            return new CoreContext(
                registry,
                stateRoot,
                heartbeatManager
                );
        }
        
    }
}