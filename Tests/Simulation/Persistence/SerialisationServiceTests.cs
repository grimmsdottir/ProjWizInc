using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions;
using ProjWizInc.Engine.Data.Definitions.Defs;
using ProjWizInc.Engine.Data.Definitions.Specifications;
using ProjWizInc.Engine.Data.Entries;
using ProjWizInc.Engine.Simulation.Bootstrapper;
using ProjWizInc.Engine.Simulation.Persistence;
using ProjWizInc.Engine.Simulation.Registries;
using ProjWizInc.Engine.Tests.TestGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Tests.Simulation.Persistence {
    public class SerialisationServiceTests {
        [Fact]
        public void Serialiser_CanSerialiserAndDeserialise() {
            // Arrange
            //create a job that is linked to gold
            ResourceEntry resourceEntry = new ResourceEntry();
            resourceEntry.Key = "gold";
            resourceEntry.Amount = new BigNum(100);
            
            ResourceEntry[] resourceEntries = new ResourceEntry[1];
            resourceEntries[0] = resourceEntry;
            PayoutSpec payoutSpec = new PayoutSpec(resourceEntries);
            JobDef jobDef = new JobDef();
            jobDef.Key = "mine";
            jobDef.Specifications.Add(payoutSpec);
            TickProgressionSpec tickProgressionSpec = new TickProgressionSpec(100);
            jobDef.Specifications.Add(tickProgressionSpec);
            //create the catalog/registry
            DefinitionCatalog catalog = new DefinitionCatalog();
            List<JobDef> jobs = new List<JobDef>() {
                jobDef
            };
            List<ResourceDef> resources = new List<ResourceDef>() {
                new ResourceDef("gold")
            };
            JobTypeHandler jobTypeHandler = new JobTypeHandler();
            ResourceTypeHandler resourceTypeHandler = new ResourceTypeHandler();
            jobTypeHandler.SetList(catalog, jobs);
            resourceTypeHandler.SetList(catalog, resources);
            DefinitionRegistry registry = new DefinitionRegistry();

            SerialisationService serialisationService = new SerialisationService();
            string json = serialisationService.Serialize(catalog);

            Assert.NotNull(json);

            DefinitionCatalog catalog2 = serialisationService.Deserialize<DefinitionCatalog>(json);
            Assert.NotNull(catalog2);
            JobDef jobDef2 = catalog2.jobDefs[0];
            ResourceDef resourceDef2 = catalog2.resourceDefs[0];
            Assert.NotNull(jobDef2);
            Assert.NotNull(resourceDef2);
            Assert.Equal("mine",jobDef2.Key);
            Assert.Equal("gold", resourceDef2.Key);
            TickProgressionSpec tick2 = jobDef2.GetSpecification<TickProgressionSpec>();
            PayoutSpec payoutSpec2 = jobDef2.GetSpecification<PayoutSpec>();
            Assert.NotNull(payoutSpec2);
            Assert.NotNull(tick2);
            Assert.Equal(100, tick2.FinishTicks);
            ResourceEntry resourceEntry2 = payoutSpec2.PayoutEntries[0];
            Assert.NotNull(resourceEntry2);
            Assert.Equal("gold",resourceEntry2.Key);
            Assert.Equal(new BigNum(100), resourceEntry2.Amount);
        }
        private class StubBigNumThing {
            public BigNum Num { get; set; }
            public StubBigNumThing(BigNum num) {
                Num = num;
            }
        }
        [Theory]
        // Small High
        [InlineData("123")] // normal number
        [InlineData("1.23e4")] //sci number
        [InlineData("123.456")] //decimal number // failed here
        [InlineData("-123")] // negative normal
        [InlineData("-1.23e4")] //negative sci
        [InlineData("-123.456")] //negative decimal // failed here
        // Below High Boundry
        [InlineData("999999999999999")] // normal number // failed here
        [InlineData("9.99e14")] //sci number
        [InlineData("999999999999999.999")] //decimal number
        [InlineData("-999999999999999")] // negative normal // failed here
        [InlineData("-9.99e14")] //negative sci
        [InlineData("-999999999999999.999")] //negative decimal
        // At High Boundry
        [InlineData("1000000000000000")] // normal number
        [InlineData("1.0e15")] //sci number
        [InlineData("1000000000000000.00")] //decimal number
        [InlineData("-1000000000000000")] // negative normal
        [InlineData("-1.0e15")] //negative sci
        [InlineData("-1000000000000000.00")] //negative decimal
        // Above High Boundry
        [InlineData("1000000000000001")] // normal number
        [InlineData("1.1e15")] //sci number
        [InlineData("1000000000000000.01")] //decimal number
        [InlineData("-1000000000000001")] // negative normal
        [InlineData("-1.1e15")] //negative sci
        [InlineData("-1000000000000000.01")] //negative decimal
        // Big High
        [InlineData("123456789123456789")] // normal number
        [InlineData("1.23e456")] //sci number
        [InlineData("123456789123456789.123")] //decimal number
        [InlineData("-123456789123456789")] // negative normal
        [InlineData("-1.23e456")] //negative sci
        [InlineData("-123456789123456789.123")] //negative decimal
        // Normal Low
        [InlineData("4.32e-1")] //sci number
        [InlineData("0.01")] //decimal number
        [InlineData("-4.32e-1")] //negative sci
        [InlineData("-0.01")] //negative decimal
        // Above Low Boundry
        [InlineData("2.0e-3")] //sci number
        [InlineData("0.0002")] //decimal number
        [InlineData("-2.0e-3")] //negative sci
        [InlineData("-0.0002")] //negative decimal
        // At Low Boundry
        [InlineData("1.0e-4")] //sci number
        [InlineData("0.0001")] //decimal number
        [InlineData("-1.0e-4")] //negative sci
        [InlineData("-0.0001")] //negative decimal
        // Below Low Boundry
        [InlineData("1.0e-5")] //sci number
        [InlineData("0.00001")] //decimal number
        [InlineData("-1.0e-5")] //negative sci
        [InlineData("-0.00001")] //negative decimal
        // Big Low
        [InlineData("1.23e-456")] //sci number
        [InlineData("0.0000123456")] //decimal number
        [InlineData("-1.23e-456")] //negative sci
        [InlineData("-0.0000123456")] //negative decimal
        public void Serialiser_CanProcessBigNums(string num) {
            StubBigNumThing testStub = new StubBigNumThing(new BigNum(num));
            SerialisationService serialisationService = new SerialisationService();
           
            string json = serialisationService.Serialize(testStub);
            StubBigNumThing testStub2 = serialisationService.Deserialize<StubBigNumThing>(json);

            Assert.NotNull(testStub2);
            Assert.True(new BigNum(num) == testStub2.Num);
        }
        [Fact]
        public void Serialiser_CanWriteAndReadFile() {
            //create a resource entry for 100 gold
            ResourceEntry resourceEntry = new ResourceEntry();
            resourceEntry.Key = "gold";
            resourceEntry.Amount = new BigNum(100);
            ResourceEntry[] resourceEntries = new ResourceEntry[1];
            resourceEntries[0] = resourceEntry;
            //create a payout spec that awards 100 gold
            PayoutSpec payoutSpec = new PayoutSpec(resourceEntries);
            //create a tick spec that completes in 100 ticks
            TickProgressionSpec tickProgressionSpec = new TickProgressionSpec(100);
            //create mine job that takes 100 ticks and awards 100 gold
            JobDef jobDef = new JobDef("mine");
            jobDef.Specifications.Add(payoutSpec);
            jobDef.Specifications.Add(tickProgressionSpec);
            //create the catalog/registry
            DefinitionCatalog catalog = new DefinitionCatalog();
            List<JobDef> jobs = new List<JobDef>() {
                jobDef
            };
            ResourceDef resourceDef = new ResourceDef("gold");
            List<ResourceDef> resources = new List<ResourceDef>() {
                resourceDef
            };
            JobTypeHandler jobTypeHandler = new JobTypeHandler();
            ResourceTypeHandler resourceTypeHandler = new ResourceTypeHandler();
            jobTypeHandler.SetList(catalog, jobs);
            resourceTypeHandler.SetList(catalog, resources);

            SerialisationService service = new SerialisationService();
            string json = service.Serialize(catalog);

            string folder = "TempTests";
            string filename = Path.Combine(folder, "tempTest.json");
            
            try {
                //Act
                Directory.CreateDirectory(folder);
                File.WriteAllText(filename, json);
                DefinitionCatalog catalog2 = service.Load<DefinitionCatalog>(filename);

                //Assert catalog
                Assert.NotNull(catalog2);
                ResourceDef resourceDef2 = catalog2.resourceDefs[0];
                Assert.NotNull(resourceDef2);
                Assert.Equal(resourceDef.Key, resourceDef2.Key);
                //check job
                JobDef jobDef2 = catalog2.jobDefs[0];
                Assert.NotNull(jobDef2);
                Assert.Equal(jobDef.Key,jobDef2.Key);
                TickProgressionSpec tick2 = jobDef2.GetSpecification<TickProgressionSpec>();
                Assert.NotNull(tick2);
                PayoutSpec payoutSpec2 = jobDef2.GetSpecification<PayoutSpec>();
                Assert.NotNull(payoutSpec2);
                ResourceEntry resourceEntry2 = payoutSpec2.PayoutEntries[0];
                Assert.NotNull(resourceEntry2);
                Assert.Equal(resourceEntry.Key, resourceEntry2.Key);
                Assert.True(resourceEntry.Amount == resourceEntry2.Amount);
            } finally {
                //Cleanup
                if (File.Exists(filename)) File.Delete(filename);
                if (Directory.Exists(folder)) Directory.Delete(folder, recursive: true);
            }
        }
        private class TestStub {
            public int num {get; set;}
            public TestStub(int num) {
                this.num = num; 
            } 
        }
        [Fact]
        public void Serialiser_ReturnsNullWhenLoadingMissingFile() {
            SerialisationService serialisationService = new SerialisationService();
            string folder = "TempTests";
            string filename = Path.Combine(folder, "doesNotExist.json");
            TestStub testStub = serialisationService.Load<TestStub>(filename);
            Assert.Null(testStub);
        }
        [Fact]
        public void Serialiser_ThrowsWhenNullInput() {
            string nullString = null;
            SerialisationService serialisationService = new SerialisationService();
            Action action = () => serialisationService.Deserialize<TestStub>(nullString);
            Assert.Throws<InvalidDataException>(action);
        }
        [Fact]
        public void Serialiser_ThrowsUnrecognisedSpecifications() {
            string unrecognisedJobSpec = """
            {
                "jobDefs": [{
                    "Specifications": [{
                        "$type": "unrecognisedSpec"
                    }]
                }]
            }
            """;
            SerialisationService serialisationService = new SerialisationService();
            DefinitionCatalog catalog = new DefinitionCatalog();
            Action action = () => catalog = serialisationService.Deserialize<DefinitionCatalog>(unrecognisedJobSpec);
            Assert.Throws<JsonException>(action);
        }
        [Fact]
        public void Serialiser_ThrowsMalformedJson() {
            string malformedJson = """
            {
                "jobDefs": [{
                    "Specifications": [{
                        "$type": "unrecognisedSpec"
            """;
            SerialisationService serialisationService = new SerialisationService();
            DefinitionCatalog catalog = new DefinitionCatalog();
            Action action = () => catalog = serialisationService.Deserialize<DefinitionCatalog>(malformedJson);
            Assert.Throws<JsonException>(action);
        }
    }
}
