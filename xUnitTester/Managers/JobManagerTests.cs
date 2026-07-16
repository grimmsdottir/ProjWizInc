using ProjWizInc.Core.Managers;
using ProjWizInc.Core.States.Managers;
using ProjWizInc.Core.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjWizInc.Core.Definitions;
using xUnitTester;
using ProjWizInc.Core.Events;

namespace xUnitTester.Managers {
    
    public class JobManagerTests {
        private bool _eventFired = false;
        private void OnResourceGained(ResourceGainedEvent e) {
            _eventFired = true;
        }
        [Fact]
        public void JobManager_Update_CompletesJob_WhenTicksReached() {
            EventManager eventManager = new EventManager();
            JobState jobState = new JobState();

            jobState.ActiveJobID = 0;
            jobState.Ticks = new BigNum(99);

            DefinitionManager definitionManager = TestHelpers.CreateTestingDefinitionManager();
            JobManager jobManager = new JobManager(eventManager,jobState,definitionManager);

            _eventFired = false;
            eventManager.Subscribe<ResourceGainedEvent>(OnResourceGained);
            for (int i = 0; i < 100; i++) {
                eventManager.Publish<UpdateLogicEvent>(new UpdateLogicEvent());
            }
            Assert.True(_eventFired, "The ResourceGainedEvent was never published!");



        }
    }
}
