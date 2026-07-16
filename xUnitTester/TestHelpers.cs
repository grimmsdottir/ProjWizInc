using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Definitions.Components;
using ProjWizInc.Core.Definitions.Entries;
using ProjWizInc.Core.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTester {
    public class TestHelpers {
        public static DefinitionManager CreateTestingDefinitionManager() {
            BigNum AMOUNT = new BigNum(69.67);
            //to build a DefinitionManager, we need a whole bunch of stuff
            //first are the dicts
            Dictionary<Type, Dictionary<string, int>> typeKeyIdMap = [];
            Dictionary<Type, Array> typeIdDefMap = [];
            //manually create some testing definitions
            ResourceDefinition testGold = new ResourceDefinition();
            testGold.Key = "gold";
            testGold.Id = 0;
            //jobs are quite a bit more complex because of their components
            JobDefinition testJob = new JobDefinition();
            testJob.Key = "mine";
            testJob.Id = 0;
            PayoutComponent testPayout = new PayoutComponent();
            ResourcePayoutEntry testPayoutEntry = new ResourcePayoutEntry();
            //we also link the components here manually
            testPayoutEntry.ResourceKey = testGold.Key;
            testPayoutEntry.ResourceID = testGold.Id;
            testPayoutEntry.Amount = AMOUNT;
            RequiresTicksComponent testTicks = new RequiresTicksComponent();
            testTicks.RequiredTicks = 100;
            testJob.Components.Add(testPayout);
            testJob.Components.Add(testTicks);

            //TODO: other definitions for job etc
            ResourceDefinition[] resourceIdDefMap = new ResourceDefinition[1];
            resourceIdDefMap[0] = testGold;
            Dictionary<string, int> resourceKeyIdMap= [];
            resourceKeyIdMap.Add(testGold.Key, testGold.Id);
            JobDefinition[] jobIdDefMap = new JobDefinition[1];
            jobIdDefMap[0] = testJob;
            Dictionary<string, int> jobKeyIdMap = [];
            jobKeyIdMap.Add(testJob.Key, testJob.Id);

            typeIdDefMap.Add(typeof(ResourceDefinition), resourceIdDefMap);
            typeKeyIdMap.Add(typeof(ResourceDefinition), resourceKeyIdMap);

            typeIdDefMap.Add(typeof(JobDefinition), jobIdDefMap);
            typeKeyIdMap.Add(typeof(JobDefinition), jobKeyIdMap);

            return new DefinitionManager(typeKeyIdMap, typeIdDefMap);
        }
    }
}
