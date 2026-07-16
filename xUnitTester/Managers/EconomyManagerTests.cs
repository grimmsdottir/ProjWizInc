using ProjWizInc.Core.Managers;
using ProjWizInc.Core.States.Managers;
using ProjWizInc.Core.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjWizInc.Core.Events;

namespace xUnitTester.Managers {
    public class EconomyManagerTests {
        [Fact]
        public void AddResource_ViaEvent_UpdatesStateCorrectly() {
            EventManager eventManager = new EventManager();
            EconomyState economyState = new EconomyState();
            economyState.Resources = new BigNum[1];
            economyState.Resources[0] = new BigNum(0);

            EconomyManager economyManager = new EconomyManager(eventManager,economyState);

            BigNum amountToAdd = new BigNum(100.50);
            int goldId = 0;

            eventManager.Publish(new ResourceGainedEvent(goldId,amountToAdd));

            BigNum currentBalance = economyState.Resources[goldId];
            Assert.Equal("100.50", currentBalance.ToString());
            
        }
    }
}
