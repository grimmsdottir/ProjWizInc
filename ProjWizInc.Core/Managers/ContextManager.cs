using ProjWizInc.Core.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    public class ContextManager {
        private readonly EventBroker _event;
        private readonly TimeManager _time;
        private readonly ProfileManager _profile;
        private readonly GameLoopManager _gameLoop;
        public static ContextManager Instance { get; } = new ContextManager();
        private ContextManager() { 
            _event = new EventBroker();
            _time = new TimeManager(_event);
            _profile = new ProfileManager(_event);
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
