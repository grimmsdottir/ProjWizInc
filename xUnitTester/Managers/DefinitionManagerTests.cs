using ProjWizInc.Core.Definitions;
using xUnitTester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Definitions.Components;
using ProjWizInc.Core.Definitions.Entries;
using ProjWizInc.Core.ADT;

namespace xUnitTester.Managers {
    public class DefinitionManagerTests {
        [Fact]
        public void TestHelpers_BuildsCorrectDefinitionManager() {
            DefinitionManager definitionManager = TestHelpers.CreateTestingDefinitionManager();
            IReadOnlyDictionary<Type, Dictionary<string, int>> typeKeyIdMap = definitionManager.GetAllTypeKeyIdMaps();
            IReadOnlyDictionary< Type, Array > typeIdDefMap = definitionManager.GetAllTypeIdDefMaps();
            Assert.NotNull(typeKeyIdMap);
            Assert.NotNull(typeIdDefMap);


            JobDefinition jobDefinition = definitionManager.GetDefinition<JobDefinition>(0);
            //check if the jobdefinition is valid
            Assert.NotNull(jobDefinition);
            Assert.Equal("mine",jobDefinition.Key);
            Assert.Equal(0, jobDefinition.Id);

            PayoutComponent payoutComponent= jobDefinition.GetComponent<PayoutComponent>();
            Assert.NotNull(payoutComponent);
            Assert.NotNull(payoutComponent.PayoutEntries);
            ResourcePayoutEntry resourcePayoutEntry = payoutComponent.PayoutEntries[0];
            Assert.NotNull(resourcePayoutEntry);
            Assert.Equal("gold", resourcePayoutEntry.ResourceKey);
            Assert.Equal(0, resourcePayoutEntry.ResourceID);
            Assert.Equal(new BigNum(69.69), resourcePayoutEntry.Amount);

            ResourceDefinition resourceDefinition = definitionManager.GetDefinition<ResourceDefinition>(0);
            Assert.NotNull(resourceDefinition);
            Assert.Equal("gold",resourceDefinition.Key);
            Assert.Equal(0,resourceDefinition.Id);

        }
    }
}
