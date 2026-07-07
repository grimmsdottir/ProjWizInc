using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core {
    public class GameLoopManager {
        public const double TimePerTick = 1.0 / 60.0;
        private double _timeAccumalator = 0;
        private readonly TaskLogic _taskLogic;
        private readonly EventBroker _eventBroker;

        public GameLoopManager(TaskLogic taskLogic, EventBroker eventBroker) {
            _taskLogic = taskLogic;
            _eventBroker = eventBroker;
        }
        public void Update(double realSecondsPassed, double speedMult) {
            if (realSecondsPassed > 0.25) {
                realSecondsPassed = 0.25;
            }
            _timeAccumalator += realSecondsPassed * speedMult;
            while (_timeAccumalator > 0) {
                _taskLogic.Update(TimePerTick);
                _timeAccumalator -= TimePerTick;
            }
            _eventBroker.Publish("RenderFrameRequested", null);
        }
    }
}
