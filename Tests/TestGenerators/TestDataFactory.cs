using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions;
using ProjWizInc.Engine.Data.Definitions.Defs;
using ProjWizInc.Engine.Data.Definitions.Specifications;
using ProjWizInc.Engine.Data.Entries;
using ProjWizInc.Engine.Simulation.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Tests.TestGenerators {
    public static class TestDataFactory {
        // A clean, explicit factory method to build a minimal, valid DualKeyMap
        public static DualKeyMap<T> CreateMinimalMap<T>(int size) where T : class, new() {
            string[] keys = new string[size];
            for (int i = 0; i < size; i++) {
                keys[i] = "item_" + i;
            }

            DualKeyMap<T> map = new DualKeyMap<T>(keys);

            // Populate with default populated items so the map is not empty
            for (int i = 0; i < size; i++) {
                map[i] = new T();
            }

            return map;
        }
        public static DefinitionCatalog GenerateGenericCatalog() {
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
            JobDef jobDef = new JobDef();
            jobDef.Key = "mine";
            jobDef.Specifications.Add(payoutSpec);
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
            return catalog;
        }
    }
}
