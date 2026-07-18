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
using ProjWizInc.Core.Definitions.Common;
using ProjWizInc.Core.Managers;

namespace xUnitTester {
    public class TestHelpers {
        public const string TEST_GOLD_KEY = "gold";
        public const int TEST_GOLD_ID = 0;
        public const string TEST_JOB_KEY = "mine";
        public const int TEST_JOB_ID = 0;
        public static readonly BigNum TEST_PAYOUT_AMOUNT = new BigNum(12.34);
        public static readonly BigNum TEST_REQUIRED_TICKS = new BigNum(100);
        public static ResourceDefinition CreateTestGoldResourceDefinition() {
            ResourceDefinition testGold = new ResourceDefinition();
            testGold.Key = TEST_GOLD_KEY;
            testGold.Id = TEST_GOLD_ID;
            return testGold;
        }
        public static ResourcePayoutEntry CreateTestPayoutEntry() {
            ResourceDefinition testGold = CreateTestGoldResourceDefinition();
            ResourcePayoutEntry testPayoutEntry = new ResourcePayoutEntry();
            testPayoutEntry.ResourceKey = testGold.Key;
            testPayoutEntry.ResourceID = testGold.Id;
            testPayoutEntry.Amount = TEST_PAYOUT_AMOUNT;
            return testPayoutEntry;
        }
        public static PayoutComponent CreateTestPayoutComponent() {
            PayoutComponent testPayoutComponent = new PayoutComponent();
            testPayoutComponent.PayoutEntries.Add(CreateTestPayoutEntry());
            return testPayoutComponent;
        }
        public static RequiresTicksComponent CreateTestTicksComponent() {
            RequiresTicksComponent testTicksComponent = new RequiresTicksComponent();
            testTicksComponent.RequiredTicks = TEST_REQUIRED_TICKS;
            return testTicksComponent;
        }
        public static JobDefinition CreateTestJobDefinition() {
            JobDefinition testJob = new JobDefinition();
            testJob.Key = TEST_JOB_KEY;
            testJob.Id = TEST_JOB_ID;
            testJob.Components.Add(CreateTestPayoutComponent());
            testJob.Components.Add(CreateTestTicksComponent());
            return testJob;
        }
        public static IEnumerable<object[]> GetValidDefinitions() {
            List<object[]> testData = new List<object[]>();

            ResourceDefinition resourceDef = CreateTestGoldResourceDefinition();
            testData.Add(new object[] { resourceDef });

            JobDefinition jobDef = CreateTestJobDefinition();
            testData.Add(new object[] { jobDef });

            return testData;
        }
        [Theory]
        [MemberData(nameof(GetValidDefinitions))]
        public void TestDefinition_HasValidKeyAndId(DefinitionBase definition) {
            DefinitionBase castedDef = definition;
            Assert.NotEmpty(castedDef.Key);
            Assert.True(castedDef.Id >= 0);
        }
        public static GameDefinitions CreateTestingGameDefinitions() {
            GameDefinitions gameDefinitions = new GameDefinitions();
            gameDefinitions.Resources.Add(CreateTestGoldResourceDefinition());
            gameDefinitions.Jobs.Add(CreateTestJobDefinition());
            return gameDefinitions;
        }
        [Fact]
        public void CreateTestingGameDefinitions_CreatesValidDefinitions() {
            GameDefinitions definitions = CreateTestingGameDefinitions();
            Assert.NotNull(definitions);
            Assert.NotEmpty(definitions.Resources);
            Assert.NotEmpty(definitions.Jobs);
            Assert.Equal(TEST_GOLD_KEY, definitions.Resources[0].Key);
            Assert.Equal(TEST_JOB_KEY, definitions.Jobs[0].Key);
        }
        public static DefinitionManager CreateTestingDefinitionManager() {
            BigNum AMOUNT = new BigNum(69.69);
            //to build a DefinitionManager, we need a whole bunch of stuff
            //first are the dicts
            Dictionary<Type, Dictionary<string, int>> typeKeyIdMap = [];
            Dictionary<Type, Array> typeIdDefMap = [];
            //manually create some testing definitions
            ResourceDefinition testGold = CreateTestGoldResourceDefinition();
            //jobs are quite a bit more complex because of their components
            JobDefinition testJob = CreateTestJobDefinition();

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
        /*
        public static JobManager CreateTestJobManager() {
            JobManager testJobManager = new JobManager()
        }
        */
    }
}
