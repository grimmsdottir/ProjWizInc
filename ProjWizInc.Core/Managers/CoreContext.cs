using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Common;
using ProjWizInc.Core.States.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    public class CoreContext {
        //our special managers
        private readonly EventManager _event;
        private readonly DefinitionManager _defs;
        //our normal managers
        private readonly EconomyManager _economy;
        private readonly GameLoopManager _gameLoop;
        private readonly JobManager _jobs;
        private readonly TimeManager _time;
        //we arrange our managers alphabetically, and specials first
        public CoreContext(
            DefinitionManager definitionManager,
            EventManager eventManager,
            EconomyManager economyManager,
            GameLoopManager gameLoopManager,
            JobManager jobManager,
            TimeManager timeManger
            ) {

            _defs = definitionManager; 
            _event = eventManager;
            _economy = economyManager;
            _gameLoop = gameLoopManager;
            _jobs = jobManager;
            _time = timeManger;
        }
        public void Start() {
            _gameLoop.Start();
        }
        public void Subscribe<T>(Action<T> handler) {
            _event.Subscribe(handler);
        }
        public void Unsubscribe<T>(Action<T> handler) {
            _event.Unsubscribe(handler);
        }
        public void Publish<T>(T eventArgs) {
            _event.Publish(eventArgs);
        }
        internal T GetDefinition<T>(int id) where T : DefinitionBase {
            return _defs.GetDefinition<T>(id);
        }
        internal int GetDefinitionCount<T>() where T : DefinitionBase {
            // Simply forward the request to the actual manager
            return _defs.GetDefCount<T>();
        }
        public TimeState GetTimeState() {
            return _time.State;
        }
    }
}
