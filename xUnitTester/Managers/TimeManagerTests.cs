using ProjWizInc.Core.Data;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.Managers;
using ProjWizInc.Core.States.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace xUnitTester.Managers {
    public class TimeManagerTests {
        [Fact]
        public void TimeManager_StartsAtZero() {
            EventManager eventManager = new EventManager();
            TimeState timeState = new TimeState();
            timeState.TicksElapsed = 0;
            timeState.TimeElapsed = 0;
            TimeManager timeManager = new TimeManager(eventManager,timeState);
            Assert.Equal(0, timeManager.GetTime());
            Assert.Equal(0, timeManager.State.TicksElapsed);
        }
        [Fact]
        public void TimeManager_IncrementsTicks_OnUpdateEvent() {
            EventManager eventManager = new EventManager();
            TimeState timeState = new TimeState();
            timeState.TicksElapsed = 0;
            timeState.TimeElapsed = 0;
            TimeManager timeManager = new TimeManager(eventManager, timeState);
            eventManager.Publish(new UpdateLogicEvent());
            Assert.Equal(1, timeManager.State.TicksElapsed);
            Assert.Equal(0, timeManager.GetTime());
        }
        [Fact]
        public void TimeManager_TicksRolloverToSeconds_AtTpsThreshold() {
            EventManager eventManager = new EventManager();
            TimeState timeState = new TimeState();
            timeState.TicksElapsed = 0;
            timeState.TimeElapsed = 0;

            TimeManager timeManager = new TimeManager(eventManager, timeState);

            // Publish exactly the number of ticks required for 1 second (100 ticks)
            int ticksNeeded = Globals.LOGIC_TPS;
            for (int i = 0; i < ticksNeeded; i++) {
                eventManager.Publish(new UpdateLogicEvent());
            }

            // Ticks should reset back to 0, and TimeElapsed should increment to 1
            Assert.Equal(0, timeManager.State.TicksElapsed);
            Assert.Equal(1, timeManager.State.TimeElapsed);
            Assert.Equal(1, timeManager.GetTime());
        }
    }
}
