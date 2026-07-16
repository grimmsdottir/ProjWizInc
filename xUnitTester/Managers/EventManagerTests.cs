using ProjWizInc.Core.Events;
using ProjWizInc.Core.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTester.Managers {
    public class EventManagerTests {
        private bool _heard = false;
        private void TestBroadcast(UpdateLogicEvent e) {
            _heard = true;
        }
        [Fact]
        public void EventManager_WorksInTests() {
            EventManager eventManager = new EventManager();
            _heard = false;
            eventManager.Subscribe<UpdateLogicEvent>(TestBroadcast);
            eventManager.Publish<UpdateLogicEvent>(new UpdateLogicEvent());
            Assert.True(_heard, "The event manager cant heard its own broadcast");
        }
    }
}
