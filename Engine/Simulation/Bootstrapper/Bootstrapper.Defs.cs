using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions;
using ProjWizInc.Engine.Data.Definitions.Commons;
using ProjWizInc.Engine.Data.Definitions.Defs;
using ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces;
using ProjWizInc.Engine.Simulation.Persistence;
using ProjWizInc.Engine.Simulation.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Simulation.Bootstrapper {
    /*
     * YOU YES YOU. OVER HERE. NEW DEF TYPE ENTERED HERE
     * dont forget to add them in Bootstrapper.Defs.cs.
     */
    public class JobTypeHandler : DefinitionTypeHandler<JobDef> {
        public override List<JobDef> GetList(DefinitionCatalog catalog) {
            return catalog.jobDefs;
        }

        public override void SetList(DefinitionCatalog catalog, List<JobDef> list) {
            catalog.jobDefs = list;
        }
    }

    public class ResourceTypeHandler : DefinitionTypeHandler<ResourceDef> {
        public override List<ResourceDef> GetList(DefinitionCatalog catalog) {
            return catalog.resourceDefs;
        }

        public override void SetList(DefinitionCatalog catalog, List<ResourceDef> list) {
            catalog.resourceDefs = list;
        }
    }
    //micro 'manager' interface for our definitions
    //we will need to declare the functions that will need our specific definitions to work on
    //implementation is elsewhere   
    public interface IDefinitionTypeHandler {
        void Merge(DefinitionCatalog target, DefinitionCatalog source);
        void Populate(DefinitionCatalog catalog, DefinitionRegistry registry);
        void ResolveLinks(DefinitionRegistry registry);
    }
    //concreter version of the interface above, but still abstract, only conrete when we provide the T,
    //so JobDef,ResourceDef etc
    public abstract class DefinitionTypeHandler<T> : IDefinitionTypeHandler where T : DefinitionBase, new() {
        // Abstract hooks that concrete subclasses implement to expose their lists
        public abstract List<T> GetList(DefinitionCatalog catalog);
        public abstract void SetList(DefinitionCatalog catalog, List<T> list);
        //now we implement the stuff that we had helper functions for before here
        //this one is used when reading the jsons and merging their contents together into the master catalog
        public void Merge(DefinitionCatalog target, DefinitionCatalog source) {
            List<T> sourceList = GetList(source);
            if (sourceList == null) {
                return;
            }

            List<T> targetList = GetList(target);
            if (targetList == null) {
                targetList = new List<T>();
                SetList(target, targetList);
            }

            for (int i = 0; i < sourceList.Count; i++) {
                targetList.Add(sourceList[i]);
            }
        }
        public void Populate(DefinitionCatalog catalog, DefinitionRegistry registry) {
            //list of each def instance
            List<T> defList = GetList(catalog);
            Dictionary<string, T> keyDefMap = new Dictionary<string,T>();
            foreach (T def in defList) {
                keyDefMap.Add(def.Key,def);
            }
            DualKeyMap<T> dualKeyMap = new DualKeyMap<T>(keyDefMap);
            registry.RegisterMap(dualKeyMap);
        }
        public void ResolveLinks(DefinitionRegistry registry) {
            DualKeyMap<T> defMap = registry.GetMap<T>();
            for (int i = 0; i < defMap.Length; i++) {
                T def = defMap[i];
                //handles the cases if it is the definition itself that needs linking
                if (def is IRequiresLinking basicLink) {
                    basicLink.ResolveLinks(registry);
                }
                if (def is CompositeDefinition composite) {
                    for (int j = 0; j < composite.Specifications.Count; j++) {
                        if (composite.Specifications[j] is IRequiresLinking spec) {
                            spec.ResolveLinks(registry);
                        }
                    }
                }
            }
        }
    }

    public static partial class Bootstrapper {
        /* 
         * YOU YES YOU, OVER HERE. DEFS HERES
         */
        private static readonly IDefinitionTypeHandler[] _definitionTypes = new IDefinitionTypeHandler[] {
            new JobTypeHandler(),
            new ResourceTypeHandler()
            //additonal new defs go here. dont forget to add them is DefList as well
        };

        //load defs from file
        private static DefinitionCatalog GetRawDefs() {
            DefinitionCatalog masterCatalog = new DefinitionCatalog();
            SerialisationService serialisationService = new SerialisationService();
            if (!Directory.Exists(DEFS_FOLDER)) {
                throw new JsonException("Critical Boot Failure: Unable to locate defs folder at " + DEFS_FOLDER + ". ");
            }
            string[] jsonFiles = Directory.GetFiles(DEFS_FOLDER, "*.json", SearchOption.AllDirectories);
            for (int i = 0; i < jsonFiles.Length; i++) {
                DefinitionCatalog partialCatalog = serialisationService.Load<DefinitionCatalog>(jsonFiles[i]);
                for (int j = 0; j < _definitionTypes.Length; j++) {
                    _definitionTypes[j].Merge(masterCatalog, partialCatalog);
                }
            }
            //go to each file listed in the manifest, read and parse it
            //merge the contents into the masterDefCatalog
            //profit
            return masterCatalog;
        }
        private static DefinitionRegistry CreateDefinitionRegistry(DefinitionCatalog catalog) {
            DefinitionRegistry registry = new DefinitionRegistry();
            for (int i = 0; i < _definitionTypes.Length; i++) {
                _definitionTypes[i].Populate(catalog, registry);
            }
            return registry;
        }
        private static void ResolveDefinitionRegistryLinks(DefinitionRegistry registry) {
            for (int i = 0; i < _definitionTypes.Length; i++) {
                _definitionTypes[i].ResolveLinks(registry);    
            }
        }
    }
}
