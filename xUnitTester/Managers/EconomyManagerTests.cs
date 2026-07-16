using ProjWizInc.Core.Managers;
using ProjWizInc.Core.States.Managers;
using ProjWizInc.Core.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjWizInc.Core.Events;
using System.Runtime.CompilerServices;

namespace xUnitTester.Managers {
    public class EconomyManagerTests {
        [Fact]
        public void AddResource_ViaEvent_UpdatesStateCorrectly() {
            const double AMMOUNT_TO_ADD = 100.50;
            EventManager eventManager = new EventManager();
            EconomyState economyState = new EconomyState();
            economyState.Resources = new BigNum[1];
            economyState.Resources[0] = new BigNum(0);

            EconomyManager economyManager = new EconomyManager(eventManager,economyState);

            BigNum amountToAdd = new BigNum(AMMOUNT_TO_ADD);
            int goldId = 0;

            eventManager.Publish(new ResourceGainedEvent(goldId,amountToAdd));

            BigNum currentBalance = economyState.Resources[goldId];
            bool isEqual = currentBalance == AMMOUNT_TO_ADD;
            Assert.True(isEqual, "Expected: "+ amountToAdd+", Actual: "+ currentBalance);
            
        }
    }
}
