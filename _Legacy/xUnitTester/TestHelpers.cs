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
        public static ResourceDefinition CreateTestResouceDef(string key, int id) {
            ResourceDefinition testResDef = new ResourceDefinition();
            testResDef.Key = key;
            testResDef.Id = id;
            return testResDef;
        }
        public static ResourceAdjustmentEntry CreateTestResourceAdjustmentEntry(string resKey, int resId, BigNum amount) {
            ResourceDefinition testResDef = CreateTestResouceDef(resKey, resId);
            ResourceAdjustmentEntry testResAdjEntry = new ResourceAdjustmentEntry();
            testResAdjEntry.ResourceID = resId;
            testResAdjEntry.Amount = amount;
            testResAdjEntry.ResourceKey = resKey;
            return testResAdjEntry;
        }
        public static PayoutComponent CreateTestPayoutComponent(ResourceAdjustmentEntry resourceAdjustmentEntry) {
            PayoutComponent testPayoutComponent = new PayoutComponent();
            testPayoutComponent.PayoutEntries.Add(resourceAdjustmentEntry);
            return testPayoutComponent;
        }
        public static PayoutComponent CreateTestPayoutComponent(ResourceAdjustmentEntry[] resourceAdjustmentEntries) {
            PayoutComponent testPayoutComponent = new PayoutComponent();
            foreach (ResourceAdjustmentEntry entry in resourceAdjustmentEntries) {
                testPayoutComponent.PayoutEntries.Add(entry); 
            }
            return testPayoutComponent;
        }
        public static RequiresTicksComponent CreateTestTicksComponent(BigNum ticks) {
            RequiresTicksComponent testTicksComponent = new RequiresTicksComponent();
            testTicksComponent.RequiredTicks = ticks;
            return testTicksComponent;
        }
        public static JobDefinition CreateTestJobDefinition(
            string jobKey,
            int jobId,
            List<IDefinitionComponentInterface> components
            ) {
            JobDefinition testJob = new JobDefinition();
            testJob.Id = jobId;
            testJob.Key = jobKey;
            foreach (IDefinitionComponentInterface component in components) {
                testJob.Components.Add(component);
            }
            return testJob;
        }
        public static GameDefinitions CreateTestGameDefinitions(List<>)
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
            Dictionary<Type,IDualKeyMap> defManagerData = new Dictionary<Type,IDualKeyMap>();

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



            return new DefinitionManager(defManagerData);
        }
    }
}
