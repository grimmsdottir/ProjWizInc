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
using ProjWizInc.Core.Definitions.Blueprints;

namespace xUnitTester.Managers {
    
    public class JobManagerTests {
        private bool _eventFired = false;
        private void OnResourceGained(ResourceGainedEvent e) {
            _eventFired = true;
        }
        private void JobCompleted(JobCompleted e) {
            _eventFired = true;
        }
        [Fact]
        public void JobManager_Update_CompletesJob_WhenTicksReached() {
            
            EventManager eventManager = new EventManager();
            JobState jobState = new JobState();
            jobState.ActiveJobId = 0;
            DefinitionManager definitionManager = TestHelpers.CreateTestingDefinitionManager();
            JobManager jobManager = new JobManager(eventManager, jobState, definitionManager);
            jobState.Ticks = new BigNum(99);
            _eventFired = false;
            jobState.ActiveJobId = 0;
            eventManager.Subscribe<ResourceGainedEvent>(OnResourceGained);
            eventManager.Publish(new UpdateLogicEvent());
            Assert.True(jobManager.UpdateMethodWasEntered, "The Update method was never even called!");
            Assert.Equal(new BigNum(0), jobState.Ticks);
            Assert.True(_eventFired, "The ResourceGainedEvent was never published!");
        }
        [Fact]
        public void Prove_Type_Consistency() {
            // Type used by the Test
            Type testType = typeof(UpdateLogicEvent);

            // Type used by the Core
            Type coreType = typeof(ProjWizInc.Core.Events.UpdateLogicEvent);

            // If this assertion fails, you have a duplicate file causing an assembly mismatch!
            Assert.Equal(coreType.AssemblyQualifiedName, testType.AssemblyQualifiedName);
        }
        [Fact]
        public void JobManager_ManualComplete_PublishesEvent() {
            EventManager eventManager = new EventManager();
            JobState state = new JobState();
            state.ActiveJobId = 0;
            DefinitionManager definitionManager = TestHelpers.CreateTestingDefinitionManager();
            JobManager jobManager = new JobManager(eventManager,state,definitionManager);
            _eventFired = false;
            eventManager.Subscribe<JobCompleted>(JobCompleted);
            jobManager.CompleteJob();
            Assert.True(_eventFired, "CompleteJob() was called, but no event was published");
        }
        [Fact]
        public void JobManager_CachesStateOnConstruction() {
            EventManager eventManager = new EventManager();
            JobState state = new JobState();
            state.ActiveJobId = 0;
            DefinitionManager definitionManager = TestHelpers.CreateTestingDefinitionManager();
            JobDefinition jobDefinition = definitionManager.GetDefinition<JobDefinition>(0);
            Assert.NotNull(jobDefinition);
            Assert.Equal(2, jobDefinition.Components.Count);
            JobManager jobManager = new JobManager(eventManager,state,definitionManager);
            Assert.NotNull(state.JobTicksRequired);
            Assert.NotNull(state.JobPayout);
        }
        [Fact]
        public void JobManager_IncrementsTicks_OnUpdateEvent() {
            EventManager eventManager = new EventManager();
            JobState state = new JobState();
            state.ActiveJobId = 0;
            DefinitionManager definitionManager = TestHelpers.CreateTestingDefinitionManager();
            JobManager jobManager = new JobManager(eventManager, state,definitionManager);
            state.Ticks = new BigNum(50);
            eventManager.Publish(new UpdateLogicEvent());
            Assert.Equal(new BigNum(51), state.Ticks);
        }
    }
}
