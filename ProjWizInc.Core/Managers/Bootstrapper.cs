using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.States;
using ProjWizInc.Core.States.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    /*
     * this class exists to create the ContextManager and all the other managers, so that we dont have
     * to clutter the context manager for all the init and whatnot, and just have it manage runtime stuff
     */
    public class Bootstrapper {
        public static CoreContext BuildContext() {
            //read jsons

            //TODO: for now we just make a new state each time, but later on we should try to load a save file
            GameState gameState = new GameState();
            //now we slice up the GameState into slices to pass to the managers
            EconomyState economyState = gameState.economyState;
            JobState jobState = gameState.jobState;
            TimeState timeState = gameState.timeState;
            //special managers that need to be initialised first
            EventManager eventManager = new EventManager();
            DefinitionManager definitionManager = new DefinitionManager();
            //construct basic managers
            EconomyManager economyManager = new EconomyManager(eventManager,economyState,definitionManager.GetDefCount<ResourceDefinition>());
            GameLoopManager gameLoopManager = new GameLoopManager(eventManager);
            JobManager jobManager = new JobManager(eventManager, jobState, definitionManager);
            TimeManager timeManager = new TimeManager(eventManager, timeState);
            return new CoreContext(
                definitionManager,
                eventManager,
                economyManager,
                gameLoopManager,
                jobManager,
                timeManager
                );
        }
    }
}
