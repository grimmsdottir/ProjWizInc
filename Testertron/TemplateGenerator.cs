using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions;
using ProjWizInc.Engine.Data.Definitions.Defs;
using ProjWizInc.Engine.Data.Definitions.Specifications;
using ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces;
using ProjWizInc.Engine.Data.Entries;
using ProjWizInc.Engine.Simulation.Persistence;
using ProjWizInc.Engine.Simulation.Registries;
using System.Collections.Generic;
using System.IO;

namespace ProjWizInc.Engine.Simulation.Bootstrapper {
    public static class TemplateGenerator {
        public static void CreateTestDefs() {
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
            string folderPath = "Defs";
            string fileName = Path.Combine(folderPath, "defs.json");
            File.WriteAllText(fileName, json);
        }
    }
}