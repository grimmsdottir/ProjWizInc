using ProjWizInc.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    public class ContextManager {
        private readonly EventManager _event;
        private readonly TimeManager _time;
        private readonly EconomyManager _resources;
        private readonly JobManager _jobs;
        private readonly GameLoopManager _gameLoop;
        private readonly DefinitionManager _defs;
        public static ContextManager Instance { get; } = new ContextManager();
        private ContextManager() { 
            _defs = new DefinitionManager();
            _defs.Init();
            _event = new EventManager();
            _time = new TimeManager(_event);
            _resources = new EconomyManager(_event,_defs);
            _resources.Init();
            _jobs = new JobManager(_event, _defs);
            _gameLoop = new GameLoopManager(_event);
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
        public TimeState GetTimeState() {
            return _time.State;
        }
    }
}
