using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions;
using ProjWizInc.Engine.Data.Definitions.Defs;
using ProjWizInc.Engine.Data.Definitions.Specifications;
using ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces;
using ProjWizInc.Engine.Data.Entries;
using ProjWizInc.Engine.Simulation.Bootstrapper;
using ProjWizInc.Engine.Simulation.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Tests.Simulation.Bootstrapper {
    public class BoostrapperDefTests {
        
        [Fact]
        public void Bootstrapper_MergesCatalogsCorrectly() {
            // Arrange
            DefinitionCatalog catalog1 = new DefinitionCatalog();
            List<JobDef> jobs1 = new List<JobDef>() {
                new JobDef("mine1"),
                new JobDef("chop1")
            };
            List<ResourceDef> resources1 = new List<ResourceDef>() {
                new ResourceDef("gold1"),
                new ResourceDef("wood1")
            };
            DefinitionCatalog catalog2 = new DefinitionCatalog();
            List<JobDef> jobs2 = new List<JobDef>() {
                new JobDef("mine2"),
                new JobDef("chop2")
            };
            List<ResourceDef> resources2 = new List<ResourceDef>() {
                new ResourceDef("gold2"),
                new ResourceDef("wood2")
            };

            ResourceTypeHandler resourceTypeHandler = new ResourceTypeHandler();
            JobTypeHandler jobTypeHandler = new JobTypeHandler();
            jobTypeHandler.SetList(catalog1, jobs1);
            resourceTypeHandler.SetList(catalog1, resources1);
            jobTypeHandler.SetList(catalog2, jobs2);
            resourceTypeHandler.SetList(catalog2, resources2);

            // Act
            jobTypeHandler.Merge(catalog1, catalog2);
            resourceTypeHandler.Merge(catalog1, catalog2);

            // Assert
            //check that the merge works: 2+2 = 4
            Assert.Equal(4,catalog1.jobDefs.Count);
            Assert.Equal(4, catalog1.resourceDefs.Count);
            //check the order of the stuff inside
            Assert.Equal("mine1", catalog1.jobDefs[0].Key);
            Assert.Equal("chop1", catalog1.jobDefs[1].Key);
            Assert.Equal("mine2", catalog1.jobDefs[2].Key);
            Assert.Equal("chop2", catalog1.jobDefs[3].Key);
            Assert.Equal("gold1", catalog1.resourceDefs[0].Key);
            Assert.Equal("wood1", catalog1.resourceDefs[1].Key);
            Assert.Equal("gold2", catalog1.resourceDefs[2].Key);
            Assert.Equal("wood2", catalog1.resourceDefs[3].Key);
            //check that the source catalog was not affected by the merge
            Assert.Equal(2, catalog2.jobDefs.Count);
            Assert.Equal("mine2", catalog2.jobDefs[0].Key);
            Assert.Equal("chop2", catalog2.jobDefs[1].Key);
            Assert.Equal(2, catalog2.resourceDefs.Count);
            Assert.Equal("gold2", catalog2.resourceDefs[0].Key);
            Assert.Equal("wood2", catalog2.resourceDefs[1].Key);
        }
        [Fact]
        public void Bootstrapper_PopulatesCorrectly() {
            DefinitionCatalog catalog = new DefinitionCatalog();
            List<JobDef> jobs = new List<JobDef>() {
                new JobDef("mine"),
                new JobDef("chop")
            };
            JobTypeHandler jobTypeHandler = new JobTypeHandler();
            jobTypeHandler.SetList(catalog, jobs);
            DefinitionRegistry registry = new DefinitionRegistry();

            jobTypeHandler.Populate(catalog,registry);
            DualKeyMap<JobDef> jobMap = registry.GetMap<JobDef>();

            Assert.NotNull(jobMap);
            Assert.Equal(2, jobMap.Length);
            Assert.True(jobMap.ContainsKey("mine"));
            Assert.True(jobMap.ContainsKey("chop"));
            //TODO: Verify that every key from your test list exists in the map and that the assigned integer IDs are contiguous and start at zero.
        }
        [Fact]
        public void Bootstrapper_LinksCorrectly() {
            // Arrange
            //create a job that is linked to gold
            ResourceEntry resourceEntry = new ResourceEntry();
            resourceEntry.Key = "gold";
            resourceEntry.Amount = new BigNum(100);
            PayoutSpec payoutSpec = new PayoutSpec();
            ResourceEntry[] resourceEntries = new ResourceEntry[1];
            payoutSpec.PayoutEntries = resourceEntries;
            payoutSpec.PayoutEntries[0] = resourceEntry;
            JobDef jobDef = new JobDef();
            jobDef.Key = "mine";
            jobDef.Specifications.Add(payoutSpec);
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
            resourceTypeHandler.Populate(catalog, registry);
            jobTypeHandler.Populate(catalog, registry);

            jobTypeHandler.ResolveLinks(registry);
            DualKeyMap<JobDef> testMap = registry.GetMap<JobDef>();
            Assert.NotNull(testMap);
            JobDef testJob = testMap.GetValue("mine");
            Assert.NotNull(testJob);
            PayoutSpec testSpec = testJob.GetSpecification<PayoutSpec>();
            Assert.NotNull(testSpec);
            ResourceEntry testEntry = testSpec.PayoutEntries[0];
            Assert.NotNull(testEntry);
            Assert.Equal("gold", testEntry.Key);
            Assert.Equal(new BigNum(100), testEntry.Amount);
        }
    }
}
