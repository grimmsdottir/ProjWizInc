using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTester {
    public class TestHelpers {
        public static DefinitionManager CreateTestingDefinitionManager() {
            //to build a DefinitionManager, we need a whole bunch of stuff
            //first are the dicts
            Dictionary<Type, Dictionary<string, int>> typeKeyIdMap = [];
            Dictionary<Type, Array> typeIdDefMap = [];
            //then the test definition
            ResourceDefinition testGold = new ResourceDefinition();
            testGold.Key = "gold";
            testGold.Id = 0;
            //TODO: other definitions for job etc
            ResourceDefinition[] resourceIdDefMap = new ResourceDefinition[1];
            resourceIdDefMap[0] = testGold;
            Dictionary<string, int> resourceKeyIdMap= [];
            resourceKeyIdMap.Add(testGold.Key, testGold.Id);

            typeIdDefMap.Add(typeof(ResourceDefinition), resourceIdDefMap);
            typeKeyIdMap.Add(typeof(ResourceDefinition), resourceKeyIdMap);

            return new DefinitionManager(typeKeyIdMap, typeIdDefMap);
        }
    }
}
