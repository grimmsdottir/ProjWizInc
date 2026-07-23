using ProjWizInc.Core.Data.ADT;
using ProjWizInc.Core.Data.Blueprints.Blueprints;
using ProjWizInc.Core.Data.Blueprints.Roots;
using ProjWizInc.Core.Simulation.Events;
using ProjWizInc.Core.Simulation.Handlers;
using ProjWizInc.Core.Simulation.Registries;
using ProjWizInc.Core.Simulation.Tickers;
using ProjWizInc.Core.States.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Simulation.Core {
    public class CoreContext {
        //our special managers
        private readonly EventHub _event;
        private readonly BlueprintRegistry _defs;
        //our normal managers
        private readonly EconomyHandler _economy;
        private readonly HeartbeatManager _gameLoop;
        private readonly JobTicker _jobs;
        private readonly TimeTicker _time;
        //we arrange our managers alphabetically, and specials first
        internal EventHub EventManager => _event;
        internal BlueprintRegistry DefinitionManager => _defs;
        internal EconomyHandler EconomyManager => _economy;
        internal HeartbeatManager GameLoopManager => _gameLoop;
        internal JobTicker JobManager => _jobs;
        internal TimeTicker TimeManager => _time;

        public CoreContext(
            BlueprintRegistry definitionManager,
            EventHub eventManager,
            EconomyHandler economyManager,
            HeartbeatManager gameLoopManager,
            JobTicker jobManager,
            TimeTicker timeManger
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
        internal T GetDefinition<T>(int id) where T : BlueprintBase, new(){
            return _defs.GetDefinition<T>(id);
        }
        internal int GetDefinitionCount<T>() where T : BlueprintBase {
            // Simply forward the request to the actual manager
            return _defs.GetDefCount<T>();
        }
        public int GetResourceCount() {
            return _defs.GetDefCount<ResourceBlueprint>();
        }
        public int GetJobCount() {
            return _defs.GetDefCount<JobBlueprint>();
        }

        public ResourceBlueprint GetResourceDefinition(int id) {
            return _defs.GetDefinition<ResourceBlueprint>(id);
        }

        public JobBlueprint GetJobDefinition(int id) {
            return _defs.GetDefinition<JobBlueprint>(id);
        }

        public BigNum GetResourceAmount(int id) {
            return _economy.GetResource(id);
        }

        public void ToggleJob(int id) {
            _jobs.ToggleJob(id);
        }

        public JobRecord GetJobState() {
            return _jobs.State;
        }
        public TimeRecord GetTimeState() {
            return _time.State;
        }
    }
}
