using ProjWizInc.Core.Data.ADT;
using ProjWizInc.Core.Data.Blueprints;
using ProjWizInc.Core.Data.Blueprints.Roots;
using ProjWizInc.Core.Data.Blueprints.Specifications.Interfaces;
using ProjWizInc.Core.States;
using ProjWizInc.Core.States.Records;
using System.Reflection;
using System.Collections;
using ProjWizInc.Core.Simulation.Events;
using ProjWizInc.Core.Simulation.Handlers;
using ProjWizInc.Core.Simulation.Persistence;
using ProjWizInc.Core.Simulation.Registries;
using ProjWizInc.Core.Simulation.Tickers;
using ProjWizInc.Core.Data.Blueprints.Blueprints;

namespace ProjWizInc.Core.Simulation.Core {
    /*
     * this class exists to create the ContextManager and all the other managers, so that we dont have
     * to clutter the context manager for all the init and whatnot, and just have it manage runtime stuff
     */
    public class Bootstrapper {
        public const string DEFS_DIRECTORY = "defs";
        //we split out BuildContext into 2, one for "proper" booting from file etc, and another which
        //can boot using specially provided defs and save for testing purposes
        public static CoreContext BuildContext() {
            SerialisationService serialiser = new SerialisationService();
            RecordRoot gameState = new RecordRoot();
            BlueprintCatalog masterDefinitions = new BlueprintCatalog();
            //first we scan and look for the defs directory
            if (Directory.Exists(DEFS_DIRECTORY)) {
                //we grab all the .json files in the defs directory
                string[] jsonFiles = Directory.GetFiles(DEFS_DIRECTORY, "*.json", SearchOption.AllDirectories);
                // Cache GameDefinitions properties once before entering the loop
                PropertyInfo[] properties = typeof(BlueprintCatalog).GetProperties();
                //we process each json
                for (int i = 0; i < jsonFiles.Length; i++) {
                    string filePath = jsonFiles[i];
                    //we load each json as a partial definition, which is an incomplete GameDefinition
                    BlueprintCatalog partialDefinitions = serialiser.Load<BlueprintCatalog>(filePath);
                    //reflection shinegigans to create the master definition from partial ones
                    //basically the compiler doesnt know how each GameDefinition.whatever is structured like
                    if (partialDefinitions != null) {
                        // Dynamically merge lists for any List<T> properties found
                        for (int p = 0; p < properties.Length; p++) {
                            PropertyInfo prop = properties[p];

                            // Verify if the property represents a generic list
                            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
                                IList partialList = (IList)prop.GetValue(partialDefinitions);
                                IList masterList = (IList)prop.GetValue(masterDefinitions);

                                if (partialList != null && masterList != null) {
                                    for (int itemIndex = 0; itemIndex < partialList.Count; itemIndex++) {
                                        masterList.Add(partialList[itemIndex]);
                                    }
                                }
                            }
                        }
                    }
                }
            } else {
                Directory.CreateDirectory(DEFS_DIRECTORY);
            }

            // 1. Build DefinitionManager first to serve as the ID and count authority
            BlueprintRegistry definitionManager = BuildDefinitionManager(masterDefinitions);

            // 2. HYDRATE AND ALLOCATE SAVE STATE ARRAYS BEFORE CONTEXT AND MANAGER CONSTRUCTION
            // Refactored to cleanly support the state DualKeyMap transition
            if (gameState.economyState.Resources == null) {
                // If you have completed the transition of EconomyState.Resources to DualKeyMap<BigNum>:
                DualKeyMap<ResourceBlueprint> resourceDefMap = definitionManager.GetMap<ResourceBlueprint>();
                gameState.economyState.Resources = new DualKeyMap<BigNum>(resourceDefMap.KeyIdMap);
                gameState.economyState.Resources.Reset(); // Struct values automatically initialize to BigNum(0)

                /* 
                 * NOTE: If you have NOT yet finished refactoring EconomyState.Resources to DualKeyMap<BigNum> 
                 * and are still using a raw BigNum[] array, you can revert back to this legacy block:
                 *
                 * int resourceCount = definitionManager.GetDefCount<ResourceDefinition>();
                 * gameState.economyState.Resources = new BigNum[resourceCount];
                 * for (int i = 0; i < resourceCount; i++) {
                 *     gameState.economyState.Resources[i] = new BigNum(0);
                 * }
                 */
            }

            // 3. Pass the fully initialized, non-null gameState to compile the service manager graph
            CoreContext context = BuildContext(masterDefinitions, gameState);

            return context;
        }

        public static CoreContext BuildContext(BlueprintCatalog rawData, RecordRoot gameState) {
            //now we slice up the GameState into slices to pass to the managers
            EconomyRecord economyState = gameState.economyState;
            JobRecord jobState = gameState.jobState;
            TimeRecord timeState = gameState.timeState;
            //special managers that need to be initialised first
            EventHub eventManager = new EventHub();
            //the definition manager is so special it gets its own section lol
            BlueprintRegistry definitionManager = BuildDefinitionManager(rawData);
            ResolveDictionaryLinks(definitionManager);
            //construct basic managers
            EconomyHandler economyManager = new EconomyHandler(eventManager, economyState);
            HeartbeatManager gameLoopManager = new HeartbeatManager(eventManager);
            JobTicker jobManager = new JobTicker(eventManager, jobState, definitionManager);
            TimeTicker timeManager = new TimeTicker(eventManager, timeState);
            return new CoreContext(
                definitionManager,
                eventManager,
                economyManager,
                gameLoopManager,
                jobManager,
                timeManager
                );
        }
        private static void ResolveDictionaryLinks(BlueprintRegistry definitionManager) {
            // Zero-allocation retrieval of the registry maps
            Dictionary<Type, IDualKeyMap>.ValueCollection allMaps = definitionManager.GetAllMaps();

            foreach (IDualKeyMap map in allMaps) {
                // Retrieve the contiguous array for polymorphic scanning
                Array idDefMaps = map.RawArray;

                foreach (object item in idDefMaps) {
                    CompositeBlueprint entity = item as CompositeBlueprint;
                    if (entity != null) {
                        List<ISpecification> components = entity.Components;
                        for (int i = 0; i < components.Count; i++) {
                            ISpecification component = components[i];
                            ILinkableSpec linkable = component as ILinkableSpec;
                            if (linkable != null) {
                                linkable.ResolveLinks(definitionManager);
                            }
                        }
                    }
                }
            }
        }


        // Consolidated and inlined reflection mapping pipeline
        private static BlueprintRegistry BuildDefinitionManager(BlueprintCatalog rawData) {
            Dictionary<Type, IDualKeyMap> typeMaps = new Dictionary<Type, IDualKeyMap>();
            PropertyInfo[] properties = typeof(BlueprintCatalog).GetProperties();

            for (int i = 0; i < properties.Length; i++) {
                PropertyInfo prop = properties[i];

                // Verify if the property represents a generic list mapping to a blueprint definition
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) {
                    object listValue = prop.GetValue(rawData);

                    if (listValue == null) {
                        continue;
                    }

                    // Extract the specific generic argument (e.g., ResourceDefinition)
                    Type itemType = prop.PropertyType.GetGenericArguments()[0];

                    // Dynamically resolve and invoke the open generic PopulateDictionaries helper
                    MethodInfo method = typeof(Bootstrapper).GetMethod(nameof(PopulateDictionaries), BindingFlags.Static | BindingFlags.NonPublic);
                    MethodInfo genericMethod = method.MakeGenericMethod(itemType);

                    genericMethod.Invoke(null, new object[] { listValue, typeMaps });
                }
            }

            return new BlueprintRegistry(typeMaps);
        }
        /* 
         * Constructs and populates strongly-typed DualKeyMaps from loaded configuration files.
         * The 'where T : DefinitionBase, new()' constraint transitively supports 
         * the parameterless constructor requirement of DualKeyMap.
         * It is neccesary because of reflection wizardy
         */
        private static void PopulateDictionaries<T>(
            List<T> items,
            Dictionary<Type, IDualKeyMap> typeMaps
        ) where T : BlueprintBase, new() {
            Dictionary<string, int> keyIdMap = new Dictionary<string, int>();

            // Phase 1: Establish sequential integer IDs and populate the forward mapping registry
            for (int i = 0; i < items.Count; i++) {
                items[i].Id = i;
                keyIdMap[items[i].Key] = i;
            }

            // Phase 2: Allocate the contiguous, flat runtime map
            DualKeyMap<T> map = new DualKeyMap<T>(keyIdMap);

            // Phase 3: Insert the definition instances into their mapped array positions
            for (int i = 0; i < items.Count; i++) {
                map.SetValue(i, items[i]);
            }

            // Phase 4: Register the completed map under its concrete Type key
            typeMaps[typeof(T)] = map;
        }


    }
}
